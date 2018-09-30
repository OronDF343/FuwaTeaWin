namespace FuwaTea.Audio.Metadata.Impl
{
    public class BasicNumericField : INumericField 
    {
        public BasicNumericField(uint maxValue = 0)
        {
            MaxValue = maxValue;
        }
        public uint MaxValue { get; }
        public uint? Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
