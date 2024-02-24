using System;

namespace ModularFramework.Configuration
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
