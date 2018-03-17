﻿using System;

namespace FuwaTea.Extensibility.ConfigurationTemp
{
    [Obsolete("TODO", true)]
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
