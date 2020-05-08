using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
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

        public MethodInfo Method {get;set;}

        public Dictionary<string, object> DefaultArgs { get; set; } = new Dictionary<string, object>();

        public ExecutionMode Mode { get; set; } = ExecutionMode.OnDemand;

        public int Order { get; set; }

    }
}
