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
using System.Linq;
using FuwaTea.Lib.Exceptions;
using LayerFramework.Attributes;
using LayerFramework.Interfaces;

namespace LayerFramework
{
    public static class LayerFactory
    {
        private static readonly Dictionary<string, IElementFactory> Layers = new Dictionary<string, IElementFactory>();

        public static void LoadFolder(string folder, ErrorCallback errorCallback, bool loadElements = false)
        {
            var exp = from t in AssemblyLoader.GetTypesFromFolder(folder, errorCallback)
                                              .FindTypesWithAttribute<LayerDefinitionAttribute>()
                      where typeof(ILayerDefinition).IsAssignableFrom(t.Key)
                      where !t.Key.IsGenericTypeDefinition
                      where typeof(IBasicElement).IsAssignableFrom(t.Value.InterfaceType)
                      where typeof(Attribute).IsAssignableFrom(t.Value.AttributeType)
                      select t;
            foreach (var p in exp)
            {
                try
                {
                    var type = typeof(ElementFactory<IBasicElement, ElementAttribute>).GetGenericTypeDefinition()
                                                                                      .MakeGenericType(p.Value.InterfaceType,
                                                                                                       p.Value.AttributeType);
                    var layer = TryUtils.TryCreateInstance<IElementFactory>(type, p.Value.LayerName);
                    Layers.Add(p.Value.LayerName.ToLowerInvariant(), layer);
                    if (loadElements) layer.LoadFolder(folder, errorCallback);
                }
                catch (Exception e)
                {
                    errorCallback(e);
                }
            }
        }

        public static IElementFactory GetFactory(string layerName) { return Layers[layerName.ToLowerInvariant()]; }

        public static IElementFactory GetFactory(Type elementType)
        {
            return Layers.Values.First(f => f.ElementInterfaceType.IsAssignableFrom(elementType));
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
    }
}
