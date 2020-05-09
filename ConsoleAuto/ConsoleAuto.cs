//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ConsoleAuto.Attributes;
using ConsoleAuto.Exceptions;
using ConsoleAuto.Model;
using ConsoleAuto.Services;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet;
using YamlDotNet.Serialization;

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

        Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();

        
        public ConsoleAuto(string[] args = null, IServiceCollection serviceBuilder = null)
        {
            config = new ConsoleAutoConfig();
            this.args = new List<string>(args ?? new string[] { });

            this.serviceBuilder = serviceBuilder ?? new ServiceCollection();

            this.Register<ReflectionService>();
            this.Register<ConsoleService>();

            this.sp = this.serviceBuilder?.BuildServiceProvider() as IServiceProvider;

            this.reflectionService = this.sp.GetService(typeof(ReflectionService)) as ReflectionService;
            this.consoleService = this.sp.GetService(typeof(ConsoleService)) as ConsoleService;


           deserializer = new Deserializer();
            
        }

        public static ConsoleAuto Config(string[] args)
        {
            var consoleAuto = new ConsoleAuto(args);
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

        public ConsoleAuto Register<T>(T instance = null) where T : class
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

        public ConsoleAuto LoadFromType(IEnumerable<Type> typeToScan)
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

                var pars = method.GetParameters();

                var defaultArgs = new Dictionary<string, object>();

                var parImpls= new List<ParamImplementation>();
                foreach (var par in pars)
                {
                    object val = null;
                    if (par.HasDefaultValue)
                    {
                        val = par.DefaultValue;
                    }
                    else
                    {
                        val = this.reflectionService.GetDefault(par.ParameterType);
                    }
                    defaultArgs[par.Name] = val;

                    //get annotatinon

                    var parAnnotation=par.GetCustomAttributes<ParamAttribute>().FirstOrDefault() ;
                  
                    parImpls.Add(new ParamImplementation()
                    {
                        Name = parAnnotation?.Name??par.Name,
                        Alias = parAnnotation?.Alias ?? default(char),
                        Info = parAnnotation?.Info
                    });
                    
                }

                var methodInfo = reflectionService.GetMethodAnnotation<ConsoleCommandAttribute>(method);

                var methodName = methodInfo.Name ?? method.Name;

                this.config.AvailableCommands.Add(new CommandImplementation()
                {
                    Method = method,
                    DefaultArgs = defaultArgs,
                    Name = methodName,
                    IsPublic = methodInfo.IsPublic,
                    Order = methodInfo.Order,
                    Mode = methodInfo.Mode,
                    Info = methodInfo.Info,
                    Params=parImpls
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
            LoadFromType(types);
            return this;
        }

        public ConsoleAuto Default(string commandname)
        {
            this.config.DefaultCommand = commandname;
            return this;
        }
        public void LoadInputArgument()
        {
            //First argument with no -- or - is the command
            var command = args.FirstOrDefault();

            var parArgs = args.ToArray();

            if (command != null && !command.Contains("-") && !string.IsNullOrEmpty(command))
            {
                parArgs = args.Skip(1).ToArray();
            }
            else
            {
                command = this.config.DefaultCommand;
            }




            //Flush all data to Program definition

            if ("exec".Equals(command))
            {
                parArgs = args.Skip(2).ToArray();

                var yaml = File.ReadAllText(args[1]);
                this.config.programDefinition = deserializer.Deserialize<ProgramDefinition>(yaml);


            }
            else
            {
                var beforeCommands = this.config.AvailableCommands.Where(x => x.Mode == ExecutionMode.BeforeCommand).OrderBy(x => x.Order).ToList();
                var afterCommands = this.config.AvailableCommands.Where(x => x.Mode == ExecutionMode.AfterCommand).OrderBy(x => x.Order).ToList();
                var commandToExecute = this.config.AvailableCommands.Where(x => command.Equals(x.Name)).FirstOrDefault();

                var commandList = new List<CommandImplementation>();

                commandList.AddRange(beforeCommands);
                commandList.Add(commandToExecute);
                commandList.AddRange(afterCommands);

                int i = 0;
                this.config.programDefinition.Commands = new Dictionary<string, CommandDefinition>();
                commandList.ForEach(command => {
                    this.config.programDefinition.Commands.Add("command" + i, new CommandDefinition()
                    {
                        Action=command.Name,
                        Desctiption=command.Info
                        // args are  empty and taken from state only
                    });
                    i ++;
                });

                
            }



            var rawValues = consoleService.ParseInputArgs(parArgs);

            foreach (var val in rawValues)
            {
                var keyDecoded = val.Key;

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

            foreach (var command in this.config.programDefinition.Commands)
            {
                var commandDef = command.Value;
                var commandImpl = this.config.AvailableCommands.FirstOrDefault(x => x.Name == commandDef.Action);

                InvokeCommand(commandImpl, commandDef);
            }
        }

        private void RegisterAllCommands()
        {
            this.config.AvailableCommands.ForEach(command =>
            {
                if (!command.Method.IsStatic)
                {
                    this.Register(command.Method.DeclaringType, null);
                }
            });

            this.sp = this.serviceBuilder.BuildServiceProvider();
        }

        private void InvokeCommand(CommandImplementation commandImpl, CommandDefinition commandDefinition)
        {

            //argument from outiside can be aliased. now are normalized. (TODO: this can done once or is different command per command?)            
            var localArgument = new Dictionary<string, object>();

            foreach (var item in this.config.programDefinition.State)
            {
                var localKey = item.Key;
                if (localKey.Length == 1)
                {
                    var par=commandImpl.Params.FirstOrDefault(x => x.Alias.ToString().Equals(localKey, StringComparison.InvariantCultureIgnoreCase));
                    if (par != null)
                    {
                        localKey = par.Name;
                     }
                }
                localArgument[localKey] = item.Value;
            }


            object instance = null;
            if (!commandImpl.Method.IsStatic)
            {
                instance = this.sp.GetService(commandImpl.Method.DeclaringType);
            }
            var args = new List<object>();
            //merge with data

            foreach (var par in commandImpl.Method.GetParameters())
            {
                object strValue = null;
                object val = null;

                if (commandDefinition.Args!=null && commandDefinition.Args.TryGetValue(par.Name, out strValue))
                {
                    val = TryGetValueFromString(args, par, strValue);
                }
                else if (localArgument.TryGetValue(par.Name, out strValue))
                {
                    val = TryGetValueFromString(args, par, strValue);
                }
                else if (commandImpl.DefaultArgs.TryGetValue(par.Name, out strValue))
                {
                    val = TryGetValueFromString(args, par, strValue);
                }
                else
                {
                    val = par.DefaultValue ?? null;                    
                }
                args.Add(val);
            }

            commandImpl.Method.Invoke(instance, args.ToArray());
        }

        private object TryGetValueFromString(List<object> args, ParameterInfo par, object strValue)
        {
            if (strValue is string)
            {
                return this.reflectionService.GetValue(par.ParameterType, strValue as string);
            }
            
            return strValue;
        }

        public ConsoleAuto LoadCommands()
        {
            return LoadCommands(Assembly.GetEntryAssembly());
        }
    }
}