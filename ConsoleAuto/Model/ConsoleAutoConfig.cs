//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System.Collections.Generic;

namespace ConsoleAuto.Model
{
    public class ConsoleAutoConfig
    {
        public ProgramDefinition programDefinition = new ProgramDefinition();

        public List<CommandImplementation> AvailableCommands { get; set; } = new List<CommandImplementation>();

        public string DefaultCommand { get; set; } = "info";
        
        public ConsoleAutoConfig()
        {
        }
    }
}