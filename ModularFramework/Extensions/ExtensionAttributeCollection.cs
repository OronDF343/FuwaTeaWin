using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModularFramework.Extensions
{
    [Serializable]
    public class ExtensionAttributeCollection
    {
        [XmlArray(nameof(Items))]
        [XmlArrayItem(nameof(ExtensionAttribute))]
        public List<ExtensionAttribute> Items { get; } = new List<ExtensionAttribute>();
    }
}
