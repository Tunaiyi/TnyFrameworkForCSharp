namespace TnyFramework.Common.Lifecycle
{

    public interface IAppClosed : ILifecycleHandler
    {
        /// <summary>
        /// 服务关闭生命周期对象
        /// </summary>
        /// <returns></returns>
        virtual PostCloser GetPostCloser() => PostCloser.Value(GetType());

        /// <summary>
        /// 服务关闭
        /// </summary>
        void OnClosed();
    }

}
