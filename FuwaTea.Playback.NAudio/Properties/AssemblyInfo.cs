using FuwaTea.Playback.NAudio;
using ModularFramework.Attributes;

// Modular
[assembly: ModuleDefinition("NAudio", typeof(NAudioExtensionAttribute), typeof(INAudioExtension))]
[assembly: ModuleImplementation("Players")]
