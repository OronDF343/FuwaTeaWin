using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModularFramework.Attributes;
using ModularFramework.Exceptions;

namespace ModularFramework
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

        private static bool EvaluateProcessorArchitecture(ProcessorArchitecture current, [NotNull] AssemblyName extension, ErrorCallback errorCallback)
        {
            if (current == extension.ProcessorArchitecture
                || extension.ProcessorArchitecture == ProcessorArchitecture.MSIL
                || extension.ProcessorArchitecture == ProcessorArchitecture.None) return true;
            
            errorCallback(new ProcessorArchitectureMismatchException(extension, current, extension.ProcessorArchitecture));
            return false;
        }

        public static Assembly LoadAssembly(string assembly, ErrorCallback errorCallback, ProcessorArchitecture? pa = null)
        {
            // make sure that the dll has a compatible processor architecture
            if (pa == null) pa = Assembly.GetCallingAssembly().GetName().ProcessorArchitecture;
            AssemblyName n;
            try { n = AssemblyName.GetAssemblyName(assembly); }
            catch (Exception e)
            {
                errorCallback(new BadImageFormatException("Invalid executable file!", e));
                return null;
            }
            return EvaluateProcessorArchitecture(pa.Value, n, errorCallback) ? Assembly.LoadFrom(assembly) : null;
        }

        private static readonly Dictionary<string, IEnumerable<Type>> TypeCache = new Dictionary<string, IEnumerable<Type>>();

        public static IEnumerable<Type> GetTypes(string assembly, ErrorCallback errorCallback, ProcessorArchitecture? pa = null)
        {
            return GetTypes(LoadAssembly(assembly, errorCallback, pa), errorCallback);
        }

        public static IEnumerable<Type> GetTypes(Assembly a, ErrorCallback errorCallback)
        {
            if (TypeCache.ContainsKey(a.Location)) return TypeCache[a.Location];
            // get the types
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
            TypeCache.Add(a.Location, new HashSet<Type>(t ?? new Type[0]));
            return t;
        }

        public static void ClearCache()
        {
            TypeCache.Clear();
        }

        public static IEnumerable<Type> GetTypesFromFolder(string folder, Func<string, bool> fileSelector, ErrorCallback errorCallback, ProcessorArchitecture? pa = null)
        {
            if (pa == null) pa = Assembly.GetCallingAssembly().GetName().ProcessorArchitecture;
            return FolderUtils.SelectFiles(folder, fileSelector)
                              .Select(dll => GetTypes(dll, errorCallback, pa.Value))
                              .Where(ts => ts != null)
                              // ReSharper disable once PossibleMultipleEnumeration
                              .SelectMany(ts => ts);
        }

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
