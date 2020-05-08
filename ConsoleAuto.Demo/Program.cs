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
        }
    }
}
