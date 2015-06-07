using System.Collections.Generic;
using LayerFramework.Interfaces;

namespace FuwaTea.Data
{
    public interface IDataElement : IBasicElement
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> SupportedFileTypes { get; }
    }
}
