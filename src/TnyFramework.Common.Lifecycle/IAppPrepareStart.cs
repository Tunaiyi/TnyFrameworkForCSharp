namespace TnyFramework.Common.Lifecycle
{

    public interface IAppPrepareStart : ILifecycleHandler
    {
        /// <summary>
        /// 准备开始生命周期对象
        /// </summary>
        virtual PrepareStarter GetPrepareStarter() => PrepareStarter.Value(GetType());

        /// <summary>
        /// 准备启动
        /// </summary>
        void OnPrepareStart();
    }

}
