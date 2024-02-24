using System;

namespace ModularFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ModuleDefinitionAttribute : Attribute
    {
        public string ModuleName { get; set; }

        public Type AttributeType { get; set; }
        public Type InterfaceType { get; set; }

        public ModuleDefinitionAttribute(string mName, Type attrType, Type iType)
        {
            ModuleName = mName;
            AttributeType = attrType;
            InterfaceType = iType;
        }
    }
}
