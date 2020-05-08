using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAuto.Model
{
    public class ConsoleAutoConfig
    {
        public ProgramDefinition programDefinition = new ProgramDefinition();

        public List<CommandImplementation> AvailableCommands { get; set; } = new List<CommandImplementation>();

        public ConsoleAutoConfig()
        {
            
        }
    }
}
