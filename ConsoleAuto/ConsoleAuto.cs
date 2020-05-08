using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ConsoleAuto.Model;
using System.Linq;
using ConsoleAuto.Services;

namespace ConsoleAuto
{
    public class ConsoleAuto
    {
        public ConsoleAutoConfig config;

        private ReflectionService reflectionService = new ReflectionService(); //DI not used for removing dependency. Service pattern used anyway
        public ConsoleAuto()
        {
            config = new ConsoleAutoConfig();
        }


        public static ConsoleAuto Config()
        {
            return new ConsoleAuto();
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
                LoadFromClass(typeToScan);
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
    }
}
