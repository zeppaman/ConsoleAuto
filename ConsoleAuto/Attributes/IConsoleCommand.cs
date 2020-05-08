using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAuto
{
    public class ConsoleCommandAttribute : Attribute
    {
        public string Name { get; internal set; }

        public ConsoleCommandAttribute(string name = null)
        {
            Name = name;
        }

    }
}
