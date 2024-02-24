using System;
using System.Reflection;
using System.Xml.Serialization;

namespace ModularFramework.Extensions
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ExtensionAttribute : Attribute
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Author { get; set; }
        [XmlAttribute]
        public string Version { get; set; }
        [XmlAttribute]
        public string Homepage { get; set; }

        public void LoadValues(Assembly a)
        {
            if (string.IsNullOrEmpty(Name)) Name = a.GetAttribute<AssemblyTitleAttribute>().Title;
            if (string.IsNullOrEmpty(Author)) Author = a.GetAttribute<AssemblyCompanyAttribute>().Company;
            if (string.IsNullOrEmpty(Version)) Version = a.GetAttribute<AssemblyVersionAttribute>().Version;
        }
    }
}
