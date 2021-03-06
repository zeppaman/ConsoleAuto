﻿//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

namespace ConsoleAuto.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*
             Examples:
             ConsoleAuto.Demo CommandOne -t 33 --test2 false
             ConsoleAuto.Demo exec test.yaml
             ConsoleAuto.Demo
             */
            ConsoleAuto.Config(args)
                .LoadCommands()
                .Run();

            //Complex exaple
            //ConsoleAuto.Config(args)
            //   .LoadCommands() //Load all commands from entry assembly + base commands
            //   .LoadCommands(assembly) //Load from a custom command
            //   .LoadFromType(typeof(MyCommand)) //load a single command
            //   .Register<MyService>() // add a service di di container used in my commands
            //   .Register<IMyService2>(new Service2()) // add a service di di container used in my commands, with a custom implementation
            //   .Configure(config => {
            //       //hack the config here
            //   })
            //   .Run();
        }
    }
}