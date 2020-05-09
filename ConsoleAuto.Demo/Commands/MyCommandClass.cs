//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System;
using ConsoleAuto.Attributes;

namespace ConsoleAuto.Demo.Commands
{
    public class MyCommandClass
    {
        [ConsoleCommand()]
        public void CommandOne([Param(alias: 't')]int test = 23, bool test2 = true)
        {
            Console.WriteLine($"You wrote {test} + {test2}");
        }

        [ConsoleCommand(info: "This command has no argument")]
        public void CommandEmpty()
        {
            Console.WriteLine($" no args");
        }

        [ConsoleCommand(name: "Console Named", info: "This command has a different name from the method name")]
        public void CommandNamed()
        {
            Console.WriteLine($" no args");
        }

        //command with context
    }
}