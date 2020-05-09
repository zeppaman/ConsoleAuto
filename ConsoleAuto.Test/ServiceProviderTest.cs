using System;
using System.Collections.Generic;
using System.Text;
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
            serviceDescriptors.AddSingleton<ConsoleService>();

            var sp=serviceDescriptors.BuildServiceProvider();
            var console=sp.GetService<ConsoleService>();
            console.WriteLine("");
            Assert.NotNull(console);
            
        }

        [Fact]
        public void Instance()
        {
            var provider = new InternalServiceProvider();

            
            InternalServiceProvider.Services[typeof(ReflectionService)] = new ReflectionService(provider);
            InternalServiceProvider.Services[typeof(ConsoleService)] = new ConsoleService();

            var command=provider.GetService(typeof(IOCommands));
            Assert.NotNull(command);
        }
    }
}

