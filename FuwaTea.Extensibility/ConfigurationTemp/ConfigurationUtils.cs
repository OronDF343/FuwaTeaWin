﻿using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;

namespace FuwaTea.Extensibility.ConfigurationTemp
{
    [Obsolete("TODO", true)]
    public static class ConfigurationUtils
    {
        /// <summary>
        /// Gets all the configurable properties of the specified element type.
        /// </summary>
        /// <param name="element">The element type. Must be a class with the <see cref="ConfigurableElementAttribute">ConfigurableElementAttribute</see> attribute.</param>
        /// <param name="errorCallback">An error callback.</param>
        /// <returns>All the configurable properties found on the element, or null if the element is not configurable.</returns>
        private static IEnumerable<IConfigurablePropertyInfo> GetProperties(Type element, Action<Exception> errorCallback)
        {
            var t = typeof(ConfigurablePropertyInfo<object>).GetGenericTypeDefinition();
            return from p in element.GetProperties()
                   where Attribute.IsDefined(p, typeof(ConfigurablePropertyAttribute))
                   let a = p.GetAttribute<ConfigurablePropertyAttribute>()
                   let r = TryUtils.TryGetResult(() => TryUtils.TryCreateInstance<IConfigurablePropertyInfo>(t.MakeGenericType(element), p, a), errorCallback)
                   where r != null
                   select r;
        }


        // TODO Hacky temporary workaround
        public static IEnumerable<IConfigurablePropertyInfo> GetAllConfigurableProperties(IContainer c, Action<Exception> errorCallback)
        {
            return c.GetServiceRegistrations().Select(s => s.ServiceType)
                    .Where(t => Attribute.IsDefined(t, typeof(ConfigurableElementAttribute)))
                    .SelectMany(et => GetProperties(et, errorCallback));
        }
    }
}
