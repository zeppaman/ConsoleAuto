using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAuto.Model
{
    public class ParamImplementation
    {
        public string Name { get; set; }

        public char Alias { get; set; }


        public string Info { get; set; }

        public bool HasAlias { get {
                return Alias != default(char);
            } }
    }
}
