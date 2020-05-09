//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System;
using System.Collections.Generic;
using ConsoleAuto.Model;

namespace ConsoleAuto.Services
{
    public class ConsoleService
    {
        protected ReflectionService reflectionService;

        public ConsoleService(ReflectionService reflectionService)
        {
            this.reflectionService = reflectionService;
        }

        internal void WriteError(string v)
        {
            Console.WriteLine(v);
        }

        public Dictionary<string, string> ParseInputArgs(string[] args)
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
                        currentArgName = argName.Trim('-');

                        status = FSMStates.WaitForValue;
                    }
                }
                else

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

        internal T Read<T>()
        {
            var strVal = Console.ReadLine();
            return (T)this.reflectionService.GetValue(typeof(T), strVal);
        }

        internal void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        private bool IsValidArg(string argName)
        {
            return argName.StartsWith("-") ||
                   argName.StartsWith("--") ||
                   string.IsNullOrEmpty(argName);
        }
    }
}