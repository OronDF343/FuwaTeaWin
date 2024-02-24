using System;

namespace ModularFramework.Exceptions
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string module, Type modelType)
        {
            ModuleName = module;
            ElementType = modelType;
        }

        public Type ElementType { get; }

        public override string Message => $"No {ModuleName} module elements found for type {ElementType.FullName}!";

        public string ModuleName { get; }
    }
}
