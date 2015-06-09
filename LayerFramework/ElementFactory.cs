using System;
using System.Collections.Generic;
using System.Linq;
using FuwaTea.Lib.Exceptions;
using LayerFramework.Attributes;
using LayerFramework.Exceptions;
using LayerFramework.Interfaces;

namespace LayerFramework
{
    public class ElementFactory<TElement, TAttribute> : IElementFactory
        where TAttribute : ElementAttribute
    {
        public ElementFactory(string name) { LayerName = name; }

        public string LayerName { get; private set; }
        public Type ElementInterfaceType { get { return typeof(TElement); } }
        public Type AttributeType { get { return typeof(TAttribute); } }

        public void LoadFolder(string folder, ErrorCallback errorCallback)
        {
            var exp = from t in AssemblyLoader.GetTypesFromFolder(folder, errorCallback)
                                              .FindTypesByAttribute<TAttribute>()
                      where typeof(TElement).IsAssignableFrom(t)
                      where !t.IsGenericTypeDefinition
                      select t;

            foreach (var p in exp)
            {
                _elements.Add(p);
            }
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
            if (!typeof(TElement).IsAssignableFrom(elemType)) throw new InvalidOperationException(elemType.Name + " does not belong to this layer!");
            var entries = _elements.Where(e => elemType.IsAssignableFrom(e) && (selector == null || selector(e)));
            var etype = entries.FirstOrDefault();
            if (etype == default(Type)) throw new ElementNotFoundException(LayerName, elemType);

            if (_cache.ContainsKey(etype))
            {
                try { return _cache[etype] as TInterface; }
                catch (Exception ex)
                {
                    throw new ElementCreationException(LayerName, etype, ex);
                }
            }

            try
            {
                var o = TryUtils.TryCreateInstance<TElement>(etype);
                _cache.Add(etype, o);
                return o as TInterface;
            }
            catch (Exception ex)
            {
                throw new ElementCreationException(LayerName, etype, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        /// <exception cref="ElementNotFoundException"></exception>
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
        /// <exception cref="ElementNotFoundException"></exception>
        /// <exception cref="ElementCreationException"></exception>
        public IEnumerable<TInterface> GetElements<TInterface>(Func<Type, bool> selector = null, ErrorCallback errorCallback = null)
            where TInterface : class //TElement
        {
            var elemType = typeof(TInterface);
            if (!typeof(TElement).IsAssignableFrom(elemType)) throw new InvalidOperationException(elemType.Name + " does not belong to this layer!");
            var entries = _elements.Where(e => elemType.IsAssignableFrom(e)
                                                 && (selector == null || selector(e))).ToList();
            if (entries.Count < 1) throw new ElementNotFoundException(LayerName, elemType);
            foreach (var etype in entries)
            {
                if (_cache.ContainsKey(etype))
                {
                    TInterface t;
                    try { t = _cache[etype] as TInterface; }
                    catch (Exception ex)
                    {
                        var x = new ElementCreationException(LayerName, etype, ex);
                        if (errorCallback == null) throw x;
                        errorCallback(x);
                        continue;
                    }
                    yield return t;
                    continue;
                }

                TElement o;
                try
                {
                    o = TryUtils.TryCreateInstance<TElement>(etype);
                    _cache.Add(etype, o);
                }
                catch (Exception ex)
                {
                    var x = new ElementCreationException(LayerName, etype, ex);
                    if (errorCallback == null) throw x;
                    errorCallback(x);
                    continue;
                }
                yield return o as TInterface;
            }
        }
    }
}
