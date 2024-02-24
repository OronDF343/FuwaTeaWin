using System;

namespace ModularFramework.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOptionsEnumeratorAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public PropertyOptionsEnumeratorAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
