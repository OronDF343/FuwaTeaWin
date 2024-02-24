using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModularFramework.Attributes;
using ModularFramework.Configuration;
using ModularFramework.Exceptions;

namespace ModularFramework
{
    public class ElementFactory<TElement, TAttribute> : IElementFactory
        where TAttribute : ElementAttribute
    {
        public ElementFactory(ModuleDefinitionAttribute info) { ModuleInfo = info; }

        public ModuleDefinitionAttribute ModuleInfo { get; }
        public Type ElementInterfaceType => typeof(TElement);
        public Type AttributeType => typeof(TAttribute);

        public void LoadFile(string fileName, ErrorCallback errorCallback)
        {
            LoadTypes(AssemblyLoader.GetTypes(fileName, errorCallback));
        }

        public void LoadAssembly(Assembly a, ErrorCallback errorCallback)
        {
            LoadTypes(AssemblyLoader.GetTypes(a, errorCallback));
        }

        public void LoadFolder(string folder, Func<string, bool> fileSelector, ErrorCallback errorCallback)
        {
            LoadTypes(AssemblyLoader.GetTypesFromFolder(folder, fileSelector, errorCallback));
        }

        private void LoadTypes(IEnumerable<Type> types)
        {
            var exp = from t in AssemblyLoader.FindByAttribute<Type, TAttribute>(types)
                      where typeof(TElement).IsAssignableFrom(t)
                      where !t.IsGenericTypeDefinition
                      select t;

            _elements.AddRange(exp);
        }

        private readonly List<Type> _elements = new List<Type>();

        private readonly Dictionary<Type, TElement> _cache = new Dictionary<Type, TElement>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ElementNotFoundException"></exception>
        /// <exception cref="ElementCreationException"></exception>
        public TInterface GetElement<TInterface>(Func<Type, bool> selector = null)
            where TInterface : class //TElement
        {
            var elemType = typeof(TInterface);
            if (!typeof(TElement).IsAssignableFrom(elemType)) throw new InvalidOperationException(elemType.Name + " does not belong to this module!");
            var entries = _elements.Where(e => elemType.IsAssignableFrom(e) && (selector == null || selector(e)));
            var etype = entries.FirstOrDefault();
            if (etype == default(Type)) throw new ElementNotFoundException(ModuleInfo.ModuleName, elemType);

            return GetInstance(etype) as TInterface;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        /// <exception cref="ElementCreationException"></exception>
        public IEnumerable<TInterface> GetElements<TInterface>(ErrorCallback errorCallback)
            where TInterface : class //TElement
        {
            return GetElements<TInterface>(null, errorCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        /// <exception cref="ElementCreationException"></exception>
        public IEnumerable<TInterface> GetElements<TInterface>(Func<Type, bool> selector = null, ErrorCallback errorCallback = null)
            where TInterface : class //TElement
        {
            var elemType = typeof(TInterface);
            if (!typeof(TElement).IsAssignableFrom(elemType)) throw new InvalidOperationException(elemType.Name + " does not belong to this module!");
            var entries = _elements.Where(e => elemType.IsAssignableFrom(e)
                                                 && (selector == null || selector(e))).ToList();
            foreach (var etype in entries)
            {
                TInterface i;
                try
                {
                    i = GetInstance(etype) as TInterface;
                }
                catch (Exception ex)
                {
                    var x = new ElementCreationException(ModuleInfo.ModuleName, etype, ex);
                    if (errorCallback == null) throw x;
                    errorCallback(x);
                    continue;
                }
                yield return i;
            }
        }

        public object GetInstance(Type ttype)
        {
            if (!_elements.Contains(ttype)) throw new InvalidOperationException(ttype.Name + " is not a valid implementation in this module!");

            try
            {
                if (_cache.ContainsKey(ttype)) return _cache[ttype];
                var o = TryUtils.TryCreateInstance<TElement>(ttype);
                _cache.Add(ttype, o);
                OnInstanceCreated(ttype, o);
                return o;
            }
            catch (Exception ex)
            {
                throw new ElementCreationException(ModuleInfo.ModuleName, ttype, ex);
            }
        }
        public bool IsInstanceAvailable(Type ttype)
        {
            return _cache.ContainsKey(ttype);
        }

        public event EventHandler<InstanceCreatedEventArgs> InstanceCreated;

        protected virtual void OnInstanceCreated(Type t, object o)
        {
            InstanceCreated?.Invoke(this, new InstanceCreatedEventArgs(t, o));
        }

        public IEnumerable<IConfigurablePropertyInfo> GetConfigurableProperties(ErrorCallback errorCallback)
        {
            foreach (var t in _elements)
            {
                var r = ConfigurationUtils.GetProperties(t, errorCallback);
                if (r == null) continue;
                foreach (var c in r)
                {
                    if (_cache.ContainsKey(t)) c.BoundObject = _cache[t];
                    else c.SetWaitForInstance(this);
                    yield return c;
                }
            }
        }
    }
}
