﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sage.Lib.Models
{
    /// <summary>
    /// I split this member out of ICanHandle to enable implementing multiple generic versions of ICanHandle without ambiguity.
    /// </summary>
    public interface ISupportFormats
    {
        [NotNull]
        IEnumerable<string> SupportedFormats { get; }
    }
}