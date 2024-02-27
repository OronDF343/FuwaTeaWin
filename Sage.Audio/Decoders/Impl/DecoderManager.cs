﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using CSCore;
using Sage.Audio.Files;
using Sage.Extensibility.Config;
using Sage.Lib.Models;

namespace Sage.Audio.Decoders.Impl
{
    [ConfigPage(nameof(DecoderManager))]
    public class DecoderManager : ImplementationPriorityManagerBase<ITrackDecoder, IFileHandle, IWaveSource>, IDecoderManager
    {
        public DecoderManager([ImportMany] IList<ITrackDecoder> implementations)
            : base(implementations) { }

        public override string FormatOf(IFileHandle ti)
        {
            return ti.Extension;
        }
    }
}