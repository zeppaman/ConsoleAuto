//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using ConsoleAuto.Services;

namespace ConsoleAuto.Commands
{
    public class IOCommands
    {
        public ConsoleService consoleService;

        public IOCommands(ConsoleService consoleService)
        {
            this.consoleService = consoleService;
        }

        [ConsoleCommand(isPublic: false, info: "provide  this description")]
        public void WriteText(string text, bool newline)
        {
            if (newline)
            {
                this.consoleService.WriteLine(text);
            }
            else
            {
                this.consoleService.Write(text);
            }
        }
    }
}