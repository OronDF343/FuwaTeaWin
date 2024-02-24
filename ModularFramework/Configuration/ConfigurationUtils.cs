using System;
using System.Collections.Generic;
using System.Linq;

namespace ModularFramework.Configuration
{
    public static class ConfigurationUtils
    {
        /// <summary>
        /// Gets all the configurable properties of the specified element type.
        /// </summary>
        /// <param name="element">The element type. Must be a class with the <see cref="ConfigurableElementAttribute">ConfigurableElementAttribute</see> attribute.</param>
        /// <param name="errorCallback">An <see cref="ErrorCallback">ErrorCallback</see>.</param>
        /// <returns>All the configurable properties found on the element, or null if the element is not configurable.</returns>
        public static IEnumerable<IConfigurablePropertyInfo> GetProperties(Type element, ErrorCallback errorCallback)
        {
            if (!Attribute.IsDefined(element, typeof(ConfigurableElementAttribute)))
                return null;
            var t = typeof(ConfigurablePropertyInfo<object>).GetGenericTypeDefinition();
            return from p in element.GetProperties()
                   where Attribute.IsDefined(p, typeof(ConfigurablePropertyAttribute))
                   let a = p.GetAttribute<ConfigurablePropertyAttribute>()
                   let r = TryUtils.TryGetResult(() => TryUtils.TryCreateInstance<IConfigurablePropertyInfo>(t.MakeGenericType(element), p, a), errorCallback)
                   where r != null
                   select r;
        }
    }
}
