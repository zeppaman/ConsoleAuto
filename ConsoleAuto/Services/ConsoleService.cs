using System;
using System.Collections.Generic;
using System.Text;
using ConsoleAuto.Model;

namespace ConsoleAuto.Services
{
    public class ConsoleService
    {
        internal void WriteError(string v)
        {
            Console.WriteLine(v);
        }

        public Dictionary<string,string> ParseInputArgs(string[] args)
        {
            int argPos = 0;
            var status = FSMStates.WaitForArg;
            string currentArgName = "";

            var rawValues = new Dictionary<string, string>();

            while (argPos < args.Length)
            {
                if (status == FSMStates.WaitForArg)
                {
                    var argName = args[argPos];


                    if (IsValidArg(argName))
                    {
                        currentArgName = argName;

                        status = FSMStates.WaitForValue;
                    }
                }


                if (status == FSMStates.WaitForValue)
                {
                    var value = args[argPos];

                    rawValues[currentArgName] = value;
                    status = FSMStates.WaitForArg;

                }

                argPos++;
            }

            return rawValues;
        }



        private bool IsValidArg(string argName)
        {
            return argName.StartsWith("-") ||
                   argName.StartsWith("--") ||
                   string.IsNullOrEmpty(argName);
        }
    }
}
