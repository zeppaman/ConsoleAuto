using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ConsoleAuto.Model;
using System.Linq;
using ConsoleAuto.Services;
using ConsoleAuto.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAuto
{
    public class ConsoleAuto
    {
        private ConsoleAutoConfig config;
        private List<string> args;

        private ReflectionService reflectionService; //DI not used for removing dependency. Service pattern used anyway
        private ConsoleService consoleService;

        private IServiceProvider sp;
        private IServiceCollection serviceBuilder;
        public ConsoleAuto(string[] args, IServiceCollection serviceBuilder = null)
        {
            config = new ConsoleAutoConfig();
            this.args = new List<string>(args);

            this.serviceBuilder = serviceBuilder ?? new ServiceCollection();
          

            this.Register<ReflectionService>();
            this.Register<ConsoleService>();

            this.sp = this.serviceBuilder?.BuildServiceProvider() as IServiceProvider;

            this.reflectionService = this.sp.GetService(typeof(ReflectionService)) as ReflectionService;
            this.consoleService = this.sp.GetService(typeof(ConsoleService)) as ConsoleService;     


        }


        public static ConsoleAuto Config(string[] args)
        {
           

            var consoleAuto= new ConsoleAuto(args);
            return consoleAuto;

        }

        public ConsoleAuto Configure(Action<ConsoleAutoConfig> action)
        {
            if (action != null)
            {
                action.Invoke(config);
            }
            return this;
        }


        public ConsoleAuto Register<T>(T instance=null) where T:class
        {
            Register(typeof(T), instance);
           
            return this;
        }

        public ConsoleAuto Register(Type type, object instance) 
        {
            if (instance != null)
            {
                this.serviceBuilder.AddSingleton(type, instance);
            }
            else
            {
                this.serviceBuilder.AddSingleton(type);
            }

            return this;
        }

        public ConsoleAuto LoadFromClass(IEnumerable<Type> typeToScan)
        {
            foreach (var type in typeToScan)
            {
                LoadFromClass(type);
            }
            return this;
        }
        public ConsoleAuto LoadFromClass(Type typeToScan)
        {
            //find all method. Each method annotated by ConsoleCommandAttribute is added
            var methods = reflectionService.GetMethods(typeToScan, typeof(ConsoleCommandAttribute));

            methods.ForEach(method =>
            {
                //find default values

                var pars=method.GetParameters();

                var defaultArgs = new Dictionary<string, object>();

                foreach (var par in pars)
                {
                    var val = this.reflectionService.GetDefault(par.ParameterType);
                    defaultArgs[par.Name] = val;
                
                }

                var methodInfo = reflectionService.GetMethodAnnotation<ConsoleCommandAttribute>(method);

                var methodName = methodInfo.Name ?? method.Name;

                this.config.AvailableCommands.Add(new CommandImplementation()
                {
                    Method=method ,
                    DefaultArgs=defaultArgs,
                    Name=methodName,
                    IsPublic= methodInfo.IsPublic,
                    Order=methodInfo.Order,
                    Mode=methodInfo.Mode,
                    Info=methodInfo.Info

                });
            });

            return this;
        }

        public ConsoleAuto LoadCommands(IEnumerable<Assembly> assemblyToScan)
        {
            foreach (var assembly in assemblyToScan)
            {
                LoadCommands(assembly);
            }
            return this;
        }

        public ConsoleAuto LoadCommands(Assembly assembly)
        {
            var types = assembly.GetTypes();
            LoadFromClass(types);
            return this;
        }


        public void LoadInputArgument()
        {
            //First argument with no -- or - is the command
            var command = args.FirstOrDefault();
            
            var parArgs = args.ToArray();
            if (command !=null && !command.Contains("-") && !string.IsNullOrEmpty(command))
            {
                this.config.programDefinition.EntryCommand = command;
                parArgs = args.Skip(1).ToArray();
            }

           

            var rawValues = consoleService.ParseInputArgs(parArgs);

            foreach (var val in rawValues)
            {
                this.config.programDefinition.State[val.Key] = val.Value;
            }
        }


        public void Run()
        {
            LoadCommands(this.GetType().Assembly);//load all system commands
            // Console.Write("");// otherwise Console in inner services dont work. need more investigation
            this.serviceBuilder.AddSingleton<ConsoleAutoConfig>(this.config);

            RegisterAllCommands();
            LoadInputArgument();
            

            var beforeCommands = this.config.AvailableCommands.Where(x => x.Mode == ExecutionMode.BeforeCommand).OrderBy(x=>x.Order).ToList();
            var afterCommands = this.config.AvailableCommands.Where(x => x.Mode == ExecutionMode.AfterCommand).OrderBy(x => x.Order).ToList();

            var commandToExecute = this.config.AvailableCommands.Where(x => x.Name == this.config.programDefinition.EntryCommand).FirstOrDefault();

            if (commandToExecute ==null)
            {
                consoleService.WriteError("Command not found.");
                throw new TerminateException();
            }

            beforeCommands.ForEach(x =>
            {
                InvokeCommand(x);
            });

           
             InvokeCommand(commandToExecute);


            afterCommands.ForEach(x =>
            {
                InvokeCommand(x);
            });
        }

        private void RegisterAllCommands()
        {
            this.config.AvailableCommands.ForEach(command => {
                if (!command.Method.IsStatic)
                {
                    this.Register(command.Method.DeclaringType, null);
                }
            });

            this.sp = this.serviceBuilder.BuildServiceProvider();
        }

        private void InvokeCommand(CommandImplementation x)
        {

            object instance = null;
            if (!x.Method.IsStatic)
            {
                instance=this.sp.GetService(x.Method.DeclaringType);
            }
            var args = new List<object>();
            //merge with data

            foreach (var par in x.Method.GetParameters())
            {
                object strValue = null;
               
                 if (this.config.programDefinition.State.TryGetValue(par.Name, out  strValue))
                {
                    if (strValue is string)
                    {
                        var obj = this.reflectionService.GetValue(par.ParameterType, strValue as string);
                        args.Add(obj);
                    }
                    else
                    {
                        args.Add(strValue);
                    }
                }
               else if (x.DefaultArgs.TryGetValue(par.Name, out strValue))
                {
                    if (strValue is string)
                    {
                        var obj = this.reflectionService.GetValue(par.ParameterType, strValue as string);
                        args.Add(obj);
                    }
                    else
                    {
                        args.Add(strValue);
                    }
                }
                else
                {
                    args.Add(par.DefaultValue ?? null);
                }
            }

            x.Method.Invoke(instance, args.ToArray());
        }

        public ConsoleAuto LoadCommands()
        {
           return LoadCommands(Assembly.GetEntryAssembly());
        }
    }
}
