using System;
using System.Linq;
using System.Reflection;

namespace PortableRest
{

    /// <summary>
    /// Extension methods on System.Type to make deserializing XML a tad easier.
    /// </summary>
    public static class TypeExtensions
    {

        private static Type[] SimpleTypes = new Type[]
            {
                typeof (string),
                typeof (decimal),
                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (TimeSpan),
                typeof (Guid)
            };

        /// <summary>
        /// Determine whether a type is simple (String, Decimal, DateTime, etc) 
        /// or complex (i.e. custom class with public properties and methods).
        /// </summary>
        /// <see href="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
                //public static bool IsSimpleType(this TypeInfo type)
        public static bool IsSimpleType(this Type type)
        {
            var info = type.GetTypeInfo();
            return
                info.IsValueType ||
                info.IsPrimitive ||
                SimpleTypes.Contains(type);
        }

    }

}