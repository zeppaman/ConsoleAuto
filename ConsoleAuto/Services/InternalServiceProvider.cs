//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleAuto.Services
{
    public class InternalServiceProvider : IServiceProvider
    {
        public static Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            object instance = null;

            if (Services.TryGetValue(serviceType, out instance))
            {
                return instance;
            }

            var constructors = serviceType.GetConstructors();

            var registeredServices = Services.Keys.ToList();

            var construct = constructors.ToList()
                .OrderByDescending(x => x.GetParameters().Length).ToList()
                .FirstOrDefault(x => x.IsPublic && IsConstructorMatching(x, registeredServices));

            if (construct == null || construct.GetParameters().Length == 0)
            {
                Services[serviceType] = Activator.CreateInstance(serviceType);
            }

            var instances = new List<object>();

            foreach (var par in construct.GetParameters())
            {
                instances.Add(this.GetService(par.ParameterType));
            }

            instance = construct.Invoke(instances.ToArray());
            Services[serviceType] = instance;
            return instance;
        }

        private bool IsConstructorMatching(ConstructorInfo x, List<Type> types)
        {
            var pars = x.GetParameters().Select(x => x.ParameterType).ToList();

            foreach (var parType in pars)
            {
                if (!types.Any(x => x.Equals(parType)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}