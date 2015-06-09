using System;
using System.Collections.Generic;
using FuwaTea.Lib.Exceptions;

namespace LayerFramework.Interfaces
{
    public interface IElementFactory
    {
        string LayerName { get; }
        Type ElementInterfaceType { get; }
        Type AttributeType { get; }
        void LoadFolder(string folder, ErrorCallback errorCallback);
        TInterface GetElement<TInterface>(Func<Type, bool> selector = null)
            where TInterface : class;
        IEnumerable<TInterface> GetElements<TInterface>(ErrorCallback errorCallback)
            where TInterface : class;
        IEnumerable<TInterface> GetElements<TInterface>(Func<Type, bool> selector = null, ErrorCallback errorCallback = null)
            where TInterface : class;
    }
}
