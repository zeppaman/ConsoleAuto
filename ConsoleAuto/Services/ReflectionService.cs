//
// Copyright (c) 2019 Daniele Fontani (https://github.com/zeppaman/ConsoleAuto/)
// RawCMS project is released under LGPL3 terms, see LICENSE file.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ConsoleAuto.Services
{
    public class ReflectionService
    {
        public ReflectionService()
        {
        }

        public List<MethodInfo> GetMethods(Type type, Type annotatedBy)
        {
            return type.GetMethods()
                .Where(x => x.GetCustomAttributes().Any(x => x.GetType().Equals(annotatedBy)))
                .ToList();
        }

        public T GetMethodAnnotation<T>(MethodInfo method) where T : Attribute
        {
            var annotation = method.GetCustomAttributes().FirstOrDefault(x => x.GetType().Equals(typeof(T)));
            return annotation as T;
        }

        public object GetValue(Type type, string strValue)
        {
            object typedValue;

            if (strValue == null)
            {
                return null;
            }

            var extPropertyType = type;

            if (type.Name == "Nullable`1")
            {
                extPropertyType = type.GenericTypeArguments[0];
            }

            switch (extPropertyType.FullName)
            {
                case "System.Boolean":
                case "bool":
                    typedValue = bool.Parse(strValue);
                    break;

                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "int":
                    typedValue = int.Parse(strValue);
                    break;

                case "System.Guid":
                    typedValue = new Guid(strValue);
                    break;

                case "System.DateTime":
                    typedValue = DateTime.ParseExact(
                                strValue,
                                "s",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeUniversal);
                    break;

                default:
                    typedValue = strValue;
                    break;
            }

            return typedValue;
        }

        public object GetDefault(Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct), return a
            //  default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message + "\")", e);
                }
            }
            return null;
        }
    }
}