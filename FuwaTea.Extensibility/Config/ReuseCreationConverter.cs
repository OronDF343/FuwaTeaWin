using System;
using Newtonsoft.Json.Converters;

namespace FuwaTea.Extensibility.Config
{
    internal class ReuseCreationConverter<T> : CustomCreationConverter<T>
    {
        private readonly T _instance;

        public ReuseCreationConverter(T instance)
        {
            _instance = instance;
        }

        public override T Create(Type objectType)
        {
            return _instance;
        }
    }
}
