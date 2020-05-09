using System;
using System.Collections.Generic;
using System.Text;
using ConsoleAuto.Model;
using Xunit;

namespace ConsoleAuto.Test
{
    public class ProgramDefinitionTest
    {

        string program =
@"
Commands:
    welcome_step1:
       Action: welcome
       Desctiption: This is the line of text that will shown first 
       Args:
            header: my text (first line)
    welcome_step2:
       Action: welcome
       Desctiption:  In this example we do it twice, to prove we can execute commands multiple times with different args.
       Args:
           header: my text (second line)
    main_step:
       Action: CommandOne
       Desctiption: This is a custom command that diplay the test. Yes, another dummy thing.
       Args:
         text: I'm the central command output!
State:
   text: myglobal

";

        [Fact]
        public void Serialization()
        {
            ProgramDefinition pd = new ProgramDefinition();
            pd.Commands = new Dictionary<string, CommandDefinition>();
            pd.Commands.Add("stepname", new CommandDefinition()
            {
                Action = "xxd",
                Args = new Dictionary<string, object>()
                {
                    {"sss", 22 }
                }
            }); ;


            pd.State = new Dictionary<string, object>()
            {
                {"key", "value" }
            };

            var dese = new YamlDotNet.Serialization.Serializer();
            var sttr = dese.Serialize(pd);
            Assert.NotNull(sttr);
        }

        [Fact]
        public void Deserialization()
        {

            var dese = new YamlDotNet.Serialization.Deserializer();
            var obj = dese.Deserialize<ProgramDefinition>(program);
            Assert.NotNull(obj);
        }
    }
}
