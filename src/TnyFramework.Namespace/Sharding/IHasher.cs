namespace TnyFramework.Namespace.Sharding
{

    public interface IHasher<in TValue>
    {
        long Hash(TValue value, int seed, long max);

        long Hash(TValue value, int seed);

    }

}
