//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System.Collections.Generic;

namespace ConsoleAuto.Model
{
    public class ProgramDefinition
    {
        public Dictionary<string, object> State { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, CommandDefinition> Commands { get; set; }

        public string EntryCommand { get; set; } = "info";
    }
}