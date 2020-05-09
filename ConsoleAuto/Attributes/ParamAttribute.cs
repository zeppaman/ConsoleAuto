//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System;

namespace ConsoleAuto.Attributes
{
    public class ParamAttribute:Attribute
    {
        public string Name { get; set; }

        public char Alias { get; set; }


        public string Info { get; set; }


        public ParamAttribute(string name = null, char alias = default(char), string info = null)
        {
            this.Name = name;
            this.Alias = alias;
            this.Info = info;
        }

    }
}