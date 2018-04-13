using System.ComponentModel.Composition;

namespace FuwaTea.Audio.Decoders
{
    [MetadataAttribute]
    public class AudioDecoderAttribute : ExportAttribute, IAudioDecoderMetadata
    {
        public AudioDecoderAttribute(params string[] formats)
            : base(typeof(IAudioDecoder))
        {
            Formats = formats;
        }

        public string[] Formats { get; }
    }
}
