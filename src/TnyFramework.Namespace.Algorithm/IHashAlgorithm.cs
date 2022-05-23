namespace TnyFramework.Namespace.Algorithm
{

    public interface IHashAlgorithm
    {
        long Hash(string value, int seed);

        long Max { get; }
    }

}
