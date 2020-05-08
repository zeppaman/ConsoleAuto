using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAuto.Model
{
    public class ProgramDefinition
    {       

        public Dictionary<string, object> State { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, CommandDefinition> Commands { get; set; }


        public string EntryCommand { get; set; }
      
    }
}
