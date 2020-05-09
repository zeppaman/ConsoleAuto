//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System.Linq;
using System.Reflection;
using ConsoleAuto.Attributes;
using ConsoleAuto.Model;
using ConsoleAuto.Services;

namespace ConsoleAuto.Commands
{
    public class InfoCommands
    {
        protected ConsoleService consoleService;
        protected ConsoleAutoConfig config;

        public InfoCommands(ConsoleService consoleService, ConsoleAutoConfig config)
        {
            this.consoleService = consoleService;
            this.config = config;
        }

        [ConsoleCommand(name: "welcome", isPublic: false, mode: ExecutionMode.BeforeCommand)]
        public void Welcome(string header)
        {
            this.consoleService.WriteLine(header ?? $"Welcome to {Assembly.GetEntryAssembly().GetName().Name}");
        }

        [ConsoleCommand(name: "info", order: -1000)]
        public void ShowInfo(string header)
        {
            //  this.consoleService.WriteLine(header ?? $"Welcome to {Assembly.GetEntryAssembly().GetName().Name}");
            this.consoleService.WriteLine("Available Options:");

            var publicCommands = this.config.AvailableCommands.Where(x => x.IsPublic).OrderBy(x => x.Order).ToList();
            do
            {
                int i = 0;
                publicCommands.ForEach(command =>
                {
                    this.consoleService.WriteLine($"{i}- {command.Name}: {command.Info}");
                    i++;
                });

                this.consoleService.WriteLine("Enter a number to get details or any other key to exit");

                string key = this.consoleService.Read<string>();
                if (int.TryParse(key, out int intVal) && intVal >= 0 && intVal < i)
                {
                    var command = publicCommands[intVal];

                    foreach (var arg in command.DefaultArgs)
                    {
                        this.consoleService.WriteLine($"{arg.Key} (default: {arg.Value ?? "null"})");
                    }
                }
                else
                {
                    break;
                }
            }
            while (true);
        }
    }
}