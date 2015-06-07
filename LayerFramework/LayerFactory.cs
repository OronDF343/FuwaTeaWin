using System;
using System.Collections.Generic;
using System.Linq;
using FuwaTea.Common.Exceptions;
using LayerFramework.Attributes;
using LayerFramework.Interfaces;

namespace LayerFramework
{
    public static class LayerFactory
    {
        private static readonly Dictionary<string, IElementFactory> Layers = new Dictionary<string, IElementFactory>();

        public static void LoadFolder(string folder, ErrorCallback errorCallback)
        {
            var exp = from t in AssemblyLoader.GetTypesFromFolder(folder, errorCallback)
                                              .FindTypesWithAttribute<LayerDefinitionAttribute>()
                      where typeof(ILayerDefinition).IsAssignableFrom(t.Key)
                      where !t.Key.IsGenericTypeDefinition
                      where typeof(IBasicElement).IsAssignableFrom(t.Value.InterfaceType)
                      where typeof(Attribute).IsAssignableFrom(t.Value.AttributeType)
                      select t;
            // TODO: update others to IsAssignableFrom
            foreach (var p in exp)
            {
                try
                {
                    Layers.Add(p.Value.LayerName.ToLowerInvariant(),
                               TryUtils.TryCreateInstance<IElementFactory>(typeof(ElementFactory<IBasicElement,
                                                                                                 ElementAttribute>).GetGenericTypeDefinition()
                                                                                                                   .MakeGenericType(p.Value.InterfaceType,
                                                                                                                                    p.Value.AttributeType),
                                                                           p.Value.LayerName));
                }
                catch (Exception e)
                {
                    errorCallback(e);
                }
            }
        }

        public static IElementFactory GetFactory(string layerName) { return Layers[layerName.ToLowerInvariant()]; }
    }
}
