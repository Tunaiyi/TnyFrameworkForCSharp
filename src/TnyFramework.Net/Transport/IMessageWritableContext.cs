using System.Threading.Tasks;

namespace TnyFramework.Net.Transport
{

    public interface IMessageWritableContext
    {
        void SetWrittenTask(Task task);
    }

}
