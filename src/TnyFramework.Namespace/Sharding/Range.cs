namespace TnyFramework.Namespace.Sharding
{

    public class Range
    {
        public long Min { get; }

        public long Max { get; }

        public Range(long min, long max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(long value)
        {
            return Min <= value && value <= Max;
        }
    }

}
