using System;

namespace LayerFramework.Exceptions
{
    public class ElementCreationException : LayerException
    {
        public ElementCreationException(string layer, Type elementType, Exception innerException, int fallbackDisplayDepth = 1)
            : base(innerException, fallbackDisplayDepth)
        {
            _layerName = layer;
            ElementType = elementType;
        }

        public Type ElementType { get; private set; }

        public override string Message
        {
            get { return string.Format("Failed to create an instance of the {0} layer element {1}!", LayerName, ElementType.FullName); }
        }

        private readonly string _layerName;
        public override string LayerName { get { return _layerName; } }
    }
}
