using System;
using System.Collections.Generic;
using System.Text;
using ConsoleAuto.Model;

namespace ConsoleAuto
{
    public class ConsoleCommandAttribute : Attribute
    {
        public string Name { get; internal set; }
        public bool IsPublic { get; internal set; } = true;
        public int Order { get; internal set; } = 100;
        public ExecutionMode Mode { get; internal set; }

        public string Info { get; set; }

        public ConsoleCommandAttribute(string name = null, bool isPublic=true, int order=100 , ExecutionMode mode=ExecutionMode.OnDemand, string info="no info provided")
        {
            Name = name;
            this.IsPublic = isPublic;
            this.Order = order;
            this.Mode = mode;
            this.Info = Info;
        }

    }
}
