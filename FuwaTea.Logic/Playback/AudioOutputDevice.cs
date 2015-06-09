using System;
using NAudio.Wave;

namespace FuwaTea.Logic.Playback
{
    public class AudioOutputDevice : IAudioOutputDevice // TODO: implement
    {
        public Guid AsGuid()
        {
            return DirectSoundOut.DSDEVID_DefaultPlayback;
        }
    }
}
