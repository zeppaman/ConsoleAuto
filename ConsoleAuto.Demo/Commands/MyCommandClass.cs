using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAuto.Demo.Commands
{
    
    public class MyCommandClass
    {
        [ConsoleCommand()]
        public void CommandOne(int test=23, bool test2=true)
        {
            Console.WriteLine($"{test} + {test2}");
        }

        [ConsoleCommand (info:"This command has no argument")]
        public void CommandEmpty()
        {
            Console.WriteLine($" no args");
        }

        [ConsoleCommand(name:"Console Named", info: "This command has a different name from the method name")]
        public void CommandNamed()
        {
            Console.WriteLine($" no args");
        }

        //command with context

    }
}
