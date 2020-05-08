using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace ConsoleAuto.Services
{
    public class ReflectionService
    {

        public List<MethodInfo> GetMethods(Type type, Type annotatedBy)
        {
            return type.GetMethods()
                .Where(x => x.GetCustomAttributes().Any(x => x.GetType().Equals(annotatedBy)))
                .ToList();
        }

        public T GetMethodAnnotation<T>(MethodInfo method) where T:Attribute
        {
            var annotation = method.GetCustomAttributes().FirstOrDefault(x => x.GetType().Equals(typeof(T)));
            return annotation as T;
        }
    }
}
