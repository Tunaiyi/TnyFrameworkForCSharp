namespace TnyFramework.Namespace.Sharding
{

    public class SlotRange
    {
        public long Min { get; }

        public long Max { get; }

        public SlotRange(long min, long max)
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
