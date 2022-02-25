using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using TnyFramework.Net.DotNetty.Bootstrap;
using IServer = Microsoft.AspNetCore.Hosting.Server.IServer;
namespace TnyFramework.Net.DotNetty.AspNetCore
{
    public class NetServer : IServer
    {
        private readonly INettyServerGuide netServerGuide;


        public NetServer(INettyServerGuide netServerGuide, IFeatureCollection features)
        {
            this.netServerGuide = netServerGuide;
            Features = features;
        }


        public void Dispose()
        {
        }


        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken = default)
        {
            await netServerGuide.Open();
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await netServerGuide.Close();
        }


        public IFeatureCollection Features { get; }
    }
}
