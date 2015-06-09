using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FuwaTea.Lib.Collections;
using FuwaTea.Lib.Exceptions;
using LayerFramework.Attributes;
using LayerFramework.Exceptions;

namespace LayerFramework
{
    public static class AssemblyLoader
    {
        private static Type[] TryGetTypes(this Assembly a, ErrorCallback errorCallback)
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                errorCallback(ex);
                return null;
            }
        }

        private static bool EvaluateProcessorArchitecture(ProcessorArchitecture current, AssemblyName extension, ErrorCallback errorCallback)
        {
            if (current == extension.ProcessorArchitecture
                || extension.ProcessorArchitecture == ProcessorArchitecture.MSIL) return true;

            errorCallback(new ProcessorArchitectureMismatchException(extension, current, extension.ProcessorArchitecture));
            return false;
        }

        private static readonly OneToManyDictionary<string, Type> TypeCache = new OneToManyDictionary<string, Type>();

        public static IEnumerable<Type> GetTypes(string assembly, ErrorCallback errorCallback, ProcessorArchitecture? pa = null)
        {
            if (TypeCache.ContainsKey(assembly)) return TypeCache[assembly];
            // first make sure that the dll has a compatible processor architecture
            if (pa == null) pa = Assembly.GetCallingAssembly().GetName().ProcessorArchitecture;
            var n = AssemblyName.GetAssemblyName(assembly);
            if (!EvaluateProcessorArchitecture(pa.Value, n, errorCallback)) return null;
            // now load it and get the types
            var a = Assembly.LoadFrom(assembly);
            if (a == null) return null;
            IEnumerable<Type> t = a.TryGetTypes(errorCallback);
            // Check OSFilterAttribute
            if (t != null)
            {
                t = from type in t
                    let f = type.GetCustomAttributes<OSFilterAttribute>()
                    let w = f.Where(o => o.Action == FilterActions.Whitelist)
                    let b = f.Where(o => o.Action == FilterActions.Blacklist)
                    where b.All(o => !o.AppliesTo(Environment.OSVersion)) // Blacklist overrides all. Make sure we are not on any blacklist.
                    where !w.Any() || w.Any(o => o.AppliesTo(Environment.OSVersion)) // If there is no whitelist, we are implicitly OK. Otherwise, we need to be on at least one whitelist.
                    select type;
            }
            TypeCache.Add(assembly, new HashSet<Type>(t ?? new Type[0]));
            return t;
        }

        public static void ClearCache()
        {
            TypeCache.Clear();
        }

        public static IEnumerable<Type> GetTypesFromFolder(string folder, ErrorCallback errorCallback, ProcessorArchitecture? pa = null)
        {
            if (pa == null) pa = Assembly.GetCallingAssembly().GetName().ProcessorArchitecture;
            return Directory.GetFiles(folder, "*.dll")
                            .Select(dll => GetTypes(dll, errorCallback, pa.Value))
                            .Where(ts => ts != null)
                            .SelectMany(ts => ts);
        }

        public static Dictionary<Type, TAttribute> FindTypesWithAttribute<TAttribute>(this IEnumerable<Type> types) where TAttribute : Attribute
        {
            return (from t in types
                    let a = t.GetCustomAttributes(typeof(TAttribute))
                    where a.Any() && !t.IsInterface && !t.IsAbstract
                    select new KeyValuePair<Type, TAttribute>(t, a.First() as TAttribute)).ToDictionary(p => p.Key, p => p.Value);
        }

        public static IEnumerable<Type> FindTypesByAttribute<TAttribute>(this IEnumerable<Type> types) where TAttribute : Attribute
        {
            return from t in types
                   where !t.IsInterface && !t.IsAbstract && Attribute.IsDefined(t, typeof(TAttribute))
                   select t;
        }
    }
}
