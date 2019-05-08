namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicNumericField : INumericField 
    {
        public BasicNumericField(uint maxValue = 0)
        {
            MaxValue = maxValue;
        }
        public uint MaxValue { get; }
        public uint? Value { get; set; }
        public virtual void ParseFrom(string s)
        {
            Value = uint.Parse(s);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
