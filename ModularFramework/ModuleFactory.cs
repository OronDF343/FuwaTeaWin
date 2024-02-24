using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModularFramework.Attributes;
using ModularFramework.Configuration;

namespace ModularFramework
{
    public static class ModuleFactory
    {
        private static readonly Dictionary<string, IElementFactory> Modules = new Dictionary<string, IElementFactory>();

        public static void LoadFolder(string folder, Func<string, bool> fileSelector, ErrorCallback errorCallback, bool loadElements)
        {
            var exp = FolderUtils.SelectFiles(folder, fileSelector)
                                 .Select(f => AssemblyLoader.LoadAssembly(f, errorCallback))
                                 .Where(a => a != null);
            foreach (var m in exp)
            {
                LoadAssembly(m, errorCallback, loadElements);
            }
        }

        public static void LoadFile(string file, ErrorCallback errorCallback, bool loadElements)
        {
            var a = AssemblyLoader.LoadAssembly(file, errorCallback);
            if (a == null) return;
            LoadAssembly(a, errorCallback, loadElements);
        }

        public static void LoadAssembly([NotNull]Assembly a, ErrorCallback errorCallback, bool loadElements)
        {
            try
            {
                if (a.IsDefined(typeof(ModuleDefinitionAttribute)))
                    foreach (var module in a.GetAttributes<ModuleDefinitionAttribute>()
                                            .Select(CreateFactory)
                                            .Where(module => loadElements))
                        module.LoadAssembly(a, errorCallback);
                if (a.IsDefined(typeof(ModuleImplementationAttribute)))
                    foreach (var attr in a.GetAttributes<ModuleImplementationAttribute>()
                                          .Where(module => loadElements))
                        Modules[attr.ModuleName].LoadAssembly(a, errorCallback);
            }
            catch (Exception e)
            {
                errorCallback(e);
            }
        }

        private static IElementFactory CreateFactory([NotNull] ModuleDefinitionAttribute ma)
        {
            var type = typeof(ElementFactory<object, ElementAttribute>).GetGenericTypeDefinition()
                                                                       .MakeGenericType(ma.InterfaceType,
                                                                                        ma.AttributeType);
            var module = TryUtils.TryCreateInstance<IElementFactory>(type, ma);
            Modules.Add(ma.ModuleName, module);
            return module;
        }

        public static IElementFactory GetFactory(string moduleName) { return Modules[moduleName.ToLowerInvariant()]; }

        public static IElementFactory GetFactory(Type elementType)
        {
            return Modules.Values.First(f => f.ElementInterfaceType.IsAssignableFrom(elementType));
        }

        public static TInterface GetElement<TInterface>(Func<Type, bool> selector = null)
            where TInterface : class
        {
            return GetFactory(typeof(TInterface)).GetElement<TInterface>(selector);
        }

        public static IEnumerable<TInterface> GetElements<TInterface>(ErrorCallback errorCallback)
            where TInterface : class
        {
            return GetFactory(typeof(TInterface)).GetElements<TInterface>(errorCallback);
        }
        public static IEnumerable<TInterface> GetElements<TInterface>(Func<Type, bool> selector = null, ErrorCallback errorCallback = null)
            where TInterface : class
        {
            return GetFactory(typeof(TInterface)).GetElements<TInterface>(selector, errorCallback);
        }
        public static object GetInstance(Type ttype)
        {
            return GetFactory(ttype).GetInstance(ttype);
        }

        public static IEnumerable<IConfigurablePropertyInfo> GetAllConfigurableProperties(ErrorCallback errorCallback)
        {
            return Modules.Values.SelectMany(f => f.GetConfigurableProperties(errorCallback)).Distinct();
        } 
    }
}
