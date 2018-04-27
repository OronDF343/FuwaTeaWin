﻿using System;

namespace FuwaTea.Audio.Playback
{
    public class AudioDeviceNotReadyException : Exception
    {
        public AudioDeviceNotReadyException(string device)
            : base("Audio device is not ready for playback: " + device)
        {
            Device = device;
        }

        public string Device { get; }
    }
}