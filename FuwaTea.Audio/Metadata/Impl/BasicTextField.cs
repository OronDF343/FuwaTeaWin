namespace FuwaTea.Audio.Metadata.Impl
{
    public class BasicTextField : ITextField
    {
        public BasicTextField(uint maxLength = 0)
        {
            MaxLength = maxLength;
        }
        public virtual uint MaxLength { get; }
        public string Value { get; set; }
    }
}
