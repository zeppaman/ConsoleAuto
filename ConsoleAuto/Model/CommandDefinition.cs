//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System.Collections.Generic;
using System.Reflection;

namespace ConsoleAuto.Model
{
    public class CommandDefinition
    {
        public string Action { get; set; }

        public string Desctiption { get; set; }
        public Dictionary<string, object> Args { get; set; } = new Dictionary<string, object>();

    }

    public class CommandImplementation
    {
        public string Name { get; set; }

        public MethodInfo Method { get; set; }

        public Dictionary<string, object> DefaultArgs { get; set; } = new Dictionary<string, object>();

        public ExecutionMode Mode { get; set; } = ExecutionMode.OnDemand;

        public int Order { get; set; }

        public bool IsPublic { get; set; } = true;

        public string Info { get; set; }

        public List<ParamImplementation> Params { get; set; } = new List<ParamImplementation>();
    }
}