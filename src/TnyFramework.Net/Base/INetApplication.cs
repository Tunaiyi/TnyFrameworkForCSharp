using System.Collections.Generic;
using System.Threading.Tasks;

namespace TnyFramework.Net.Base
{

    public interface INetApplication
    {
        INetAppContext AppContext { get; }

        IList<INetServer> Servers { get; }

        Task Start();

        Task Close();
    }

}
