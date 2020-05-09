using System;

namespace ConsoleAuto.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
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
