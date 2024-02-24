using System;

namespace ModularFramework.Exceptions
{
    public class ElementCreationException : Exception
    {
        public ElementCreationException(string module, Type elementType, Exception innerException)
            : base("", innerException)
        {
            ModuleName = module;
            ElementType = elementType;
        }

        public Type ElementType { get; }

        public override string Message => $"Failed to create an instance of the {ModuleName} module element {ElementType.FullName}!";

        public string ModuleName { get; }
    }
}
