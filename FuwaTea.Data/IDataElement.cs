using System.Collections.Generic;
using LayerFramework.Interfaces;

namespace FuwaTea.Data
{
    public interface IDataElement : IBasicElement
    {
        /// <summary>
        /// The file types (file extensions) this element can handle. MUST INCLUDE THE DOT!
        /// </summary>
        IEnumerable<string> SupportedFileTypes { get; }
    }
}
