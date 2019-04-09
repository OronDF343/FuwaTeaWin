using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Lib.DataModel;
using Sage.Extensibility.Config;

namespace Sage.Audio.Playback
{
    [ConfigPage(nameof(ApiBasedAudioPlayer)), Export]
    public class ApiSelector : ImplementationSelectorBase<IAudioApi>
    {
        public ApiSelector([ImportMany] IList<IAudioApi> implementations)
            : base(implementations) { }
    }
}
