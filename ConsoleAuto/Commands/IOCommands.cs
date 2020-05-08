using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAuto.Commands
{
    public class IOCommands
    {
     
        [ConsoleCommand]
        public void WriteText(string text)
        {
            Console.WriteLine(text);
        }
    }
}
