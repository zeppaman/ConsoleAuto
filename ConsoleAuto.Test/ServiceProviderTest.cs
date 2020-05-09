//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using ConsoleAuto.Commands;
using ConsoleAuto.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConsoleAuto.Test
{
    public class ServiceProviderTest
    {
        [Fact]
        public void Native()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton<ReflectionService>();
            serviceDescriptors.AddSingleton<ConsoleService>();

            var sp = serviceDescriptors.BuildServiceProvider();
            var console = sp.GetService<ConsoleService>();
            console.WriteLine("");
            Assert.NotNull(console);
        }

        [Fact]
        public void Instance()
        {
            var provider = new InternalServiceProvider();

            var reflection = new ReflectionService();
            InternalServiceProvider.Services[typeof(ReflectionService)] = reflection;
            InternalServiceProvider.Services[typeof(ConsoleService)] = new ConsoleService(reflection);

            var command = provider.GetService(typeof(IOCommands));
            Assert.NotNull(command);
        }
    }
}