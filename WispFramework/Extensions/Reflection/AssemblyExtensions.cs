using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WispFramework.Extensions.Reflection
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> SubclassesOfType<TBaseType>(this Assembly asmAssembly)
        {
            var baseType = typeof(TBaseType);
            return asmAssembly.GetTypes().Where(t => baseType.IsAssignableFrom(t));
        }

        public static IEnumerable<Tuple<TAttributeType, Type>> GetTypesWithAttribute<TAttributeType>(
            this IEnumerable<Type> types) where TAttributeType : Attribute
        {
            return types.Select(t => new Tuple<TAttributeType, Type>(t.GetCustomAttributes<TAttributeType>().FirstOrDefault(), t)).Where(t => t.Item1 != null);
        }
    }
}
