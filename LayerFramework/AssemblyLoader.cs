#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

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
        private static readonly string[] ExecutableFileFilters = {"*.dll", "*.exe"};

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
            return ExecutableFileFilters.SelectMany(s => Directory.EnumerateFiles(folder, s))
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
