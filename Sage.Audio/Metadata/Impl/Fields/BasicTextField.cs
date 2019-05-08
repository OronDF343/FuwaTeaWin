namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicTextField : ITextField
    {
        public BasicTextField(uint maxLength = 0)
        {
            MaxLength = maxLength;
        }
        public virtual uint MaxLength { get; }
        public string Value { get; set; }
        public virtual void ParseFrom(string s)
        {
            Value = s;
        }
    }
}
