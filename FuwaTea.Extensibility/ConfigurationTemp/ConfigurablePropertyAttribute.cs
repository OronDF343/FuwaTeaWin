using System;

namespace FuwaTea.Extensibility.ConfigurationTemp
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurablePropertyAttribute : Attribute
    {
        public ConfigurablePropertyAttribute() { }
        public ConfigurablePropertyAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
        public object DefaultValue { get; set; }
    }
}
