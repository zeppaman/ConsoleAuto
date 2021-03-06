﻿//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System.Linq;
using ConsoleAuto.Demo.Commands;
using ConsoleAuto.Model;
using Xunit;

namespace ConsoleAuto.Demo.Test
{
    public class ConfigTest
    {
        [Fact]
        public void GetCommandsFromAnnotations()
        {
            ConsoleAutoConfig config = null;
            ConsoleAuto
                .Config(null)
                .LoadFromClass(typeof(MyCommandClass))
                .Configure(x =>
                {
                    config = x;
                });

            TestConfig(config);
        }

        [Fact]
        public void GetCommandsFromAssembly()
        {
            ConsoleAutoConfig config = null;
            ConsoleAuto
                .Config(null)
                .LoadCommands(typeof(MyCommandClass).Assembly)
                .Configure(x =>
                {
                    config = x;
                });

            TestConfig(config);
        }

        private void TestConfig(ConsoleAutoConfig config)
        {
            Assert.Equal(3, config.AvailableCommands.Count);

            Assert.True(config.AvailableCommands.Any(x => x.Name == "CommandOne"));
            Assert.True(config.AvailableCommands.Any(x => x.Name == "CommandEmpty"));
            Assert.True(config.AvailableCommands.Any(x => x.Name == "Console Named"));

            var commandOne = config.AvailableCommands.FirstOrDefault(x => x.Name == "CommandOne");
            Assert.True(commandOne.DefaultArgs.Count == 3);
            Assert.True(commandOne.DefaultArgs.ContainsKey("test"));
            Assert.True(commandOne.DefaultArgs.ContainsKey("test2"));
            Assert.True(commandOne.DefaultArgs.ContainsKey("text"));
            Assert.Equal(commandOne.DefaultArgs["test"], 23);
            Assert.Equal(commandOne.DefaultArgs["test2"], true);
        }
    }
}