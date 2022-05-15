using Microsoft.Extensions.DependencyInjection;

namespace TnyFramework.DI.Container
{

    /// <summary>
    /// 应用模块
    /// </summary>
    public interface IApplicationModule
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="collection"></param>
        void Initialize(IServiceCollection collection);

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="collection"></param>
        void Close(IServiceCollection collection);
    }

}
