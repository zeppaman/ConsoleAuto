using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ConsoleAuto.Model;
using System.Linq;
using ConsoleAuto.Services;
using ConsoleAuto.Exceptions;

namespace ConsoleAuto
{
    public class ConsoleAuto
    {
        private ConsoleAutoConfig config;
        private List<string> args;

        private ReflectionService reflectionService = new ReflectionService(); //DI not used for removing dependency. Service pattern used anyway
        private ConsoleService consoleService = new ConsoleService();
        public ConsoleAuto(string[] args)
        {
            config = new ConsoleAutoConfig();
            this.args = new List<string>(args);
        }


        public static ConsoleAuto Config(string[] args)
        {
            return new ConsoleAuto(args);
        }

        public ConsoleAuto Configure(Action<ConsoleAutoConfig> action)
        {
            if (action != null)
            {
                action.Invoke(config);
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
                    defaultArgs[par.Name] = par.DefaultValue??null;
                
                }

                var methodInfo = reflectionService.GetMethodAnnotation<ConsoleCommandAttribute>(method);

                var methodName = methodInfo.Name ?? method.Name;

                this.config.AvailableCommands.Add(new CommandImplementation()
                {
                    Method=method ,
                    DefaultArgs=defaultArgs,
                    Name=methodName

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
            if (command.Contains("-") || string.IsNullOrEmpty(command))
            {
                consoleService.WriteError("The first argument must be the command name. This argument must be a single word, without spaces or specia characters");
                throw new TerminateException();
            }

            this.config.programDefinition.EntryCommand = command;

            var rawValues = consoleService.ParseInputArgs(args.Skip(1).ToArray());

            foreach (var val in rawValues)
            {
                this.config.programDefinition.State[val.Key] = val.Value;
            }
        }


        public void Run()
        {
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

        private void InvokeCommand(CommandImplementation x)
        {
            Console.WriteLine(x.Name);
            foreach (var val in this.config.programDefinition.State)
            {
                Console.WriteLine($"{val.Key}-{val.Value}");
            }
        }

        public ConsoleAuto LoadCommands()
        {
           return LoadCommands(Assembly.GetExecutingAssembly());
        }
    }
}
