using System;
using System.Collections.Generic;
using System.Text;
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

        [ConsoleCommand(isPublic: false,info:"provide  this description")]
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
