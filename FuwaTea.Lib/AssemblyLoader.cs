using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FuwaTea.Lib
{
    public static class AssemblyLoader
    {
        public static Dictionary<TReflect, TAttribute> FindWithAttribute<TReflect, TAttribute>(IEnumerable<TReflect> types, bool inherit = true)
            where TReflect : ICustomAttributeProvider
            where TAttribute : Attribute
        {
            return (from t in types
                    let a = t.GetAttribute<TAttribute>(inherit)
                    where a != null
                    select new KeyValuePair<TReflect, TAttribute>(t, a)).ToDictionary(p => p.Key, p => p.Value);
        }

        public static Dictionary<TReflect, IEnumerable<TAttribute>> FindWithAttributes<TReflect, TAttribute>(IEnumerable<TReflect> types, bool inherit = true)
            where TReflect : ICustomAttributeProvider
            where TAttribute : Attribute
        {
            return (from t in types
                    let a = t.GetAttributes<TAttribute>(inherit)
                    where a.Any()
                    select new KeyValuePair<TReflect, IEnumerable<TAttribute>>(t, a)).ToDictionary(p => p.Key, p => p.Value);
        }

        public static IEnumerable<TReflect> FindByAttribute<TReflect, TAttribute>(IEnumerable<TReflect> types, bool inherit = true)
            where TReflect : ICustomAttributeProvider
            where TAttribute : Attribute
        {
            return from t in types
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider p, bool inherit = true)
            where TAttribute : Attribute
        {
            return p.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }
        
        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider p, bool inherit = true)
            where TAttribute : Attribute
        {
            return p.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() as TAttribute;
        }
    }
}
