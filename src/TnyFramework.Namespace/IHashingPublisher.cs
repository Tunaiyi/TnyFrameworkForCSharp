using System.Threading.Tasks;
using TnyFramework.Codec;

namespace TnyFramework.Namespace
{

    public delegate Task<NameNode<TValue>> Publishing<TValue>(
        INamespaceExplorer explorer, string path, TValue value, ObjectMimeType<TValue> mineType, ILessee lessee);

    public interface IHashingPublisher<in TKey, TValue>
    {
        string Path { get; }

        ObjectMimeType<TValue> MineType { get; }

        Task<ILessee> Lease();

        Task<ILessee> Lease(long ttl);

        string PathOf(TKey key, TValue value);

        Task<NameNode<TValue>> Publish(TKey key, TValue value);

        Task<NameNode<TValue>> Operate(TKey key, TValue value, Publishing<TValue> publishing);

        Task<NameNode<TValue>> PublishIfAbsent(TKey key, TValue value);

        Task<NameNode<TValue>> PublishIfExist(TKey key, TValue value);

        Task<NameNode<TValue>> Revoke(TKey key, TValue value);
    }

}
