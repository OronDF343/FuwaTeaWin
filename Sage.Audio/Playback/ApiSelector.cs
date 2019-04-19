﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sage.Extensibility.Config;
using Sage.Lib.Models;

namespace Sage.Audio.Playback
{
    [ConfigPage(nameof(ApiBasedAudioPlayer)), Export]
    public class ApiSelector : ImplementationSelectorBase<IAudioApi>
    {
        public ApiSelector([ImportMany] IList<IAudioApi> implementations)
            : base(implementations) { }
    }
}
