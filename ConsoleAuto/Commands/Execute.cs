//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using ConsoleAuto.Attributes;

namespace ConsoleAuto.Commands
{
    public class Execute
    {

        [ConsoleCommand(isPublic: true, name:"exec", info: "read a program definition from file and load")]
        public void Program([Param(name:"",alias:' ',info:"the file name follow the command without any  name: my.exe exec file.yaml")] string path)
        {
            //this is a special command used for doc only
        }
    }
}