using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Extensibility.Config;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Playback
{
    [ConfigPage(nameof(ApiBasedAudioPlayer))]
    public class ApiSelector : ImplementationSelectorBase<IAudioApi>
    {
        public ApiSelector([ImportMany] IList<IAudioApi> implementations)
            : base(implementations) { }
    }
}
