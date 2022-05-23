using System.Threading.Tasks;

namespace TnyFramework.Namespace.Etcd
{

    public interface IEtcdWatcher
    {
        bool IsClosed();

        Task Close();
    }

}
