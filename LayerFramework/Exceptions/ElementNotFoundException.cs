using System;

namespace LayerFramework.Exceptions
{
    public class ElementNotFoundException : LayerException
    {
        public ElementNotFoundException(string layer, Type modelType)
            : base(layer)
        {
            ElementType = modelType;
        }

        public Type ElementType { get; private set; }

        public override string Message
        {
            get { return string.Format("No {0} layer elements found for type {1}!", LayerName, ElementType.FullName); }
        }
    }
}
