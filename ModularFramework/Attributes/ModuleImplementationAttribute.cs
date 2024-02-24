using System;

namespace ModularFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ModuleImplementationAttribute : Attribute
    {
        public string ModuleName { get; set; }

        public ModuleImplementationAttribute(string name)
        {
            ModuleName = name;
        }
    }
}
