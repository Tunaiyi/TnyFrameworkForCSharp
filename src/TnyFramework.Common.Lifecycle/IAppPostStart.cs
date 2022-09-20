namespace TnyFramework.Common.Lifecycle
{

    public interface IAppPostStart : ILifecycleHandler
    {
        /// <summary>
        /// 启动完成生命周期对象
        /// </summary>
        virtual PostStarter GetPostStarter() => PostStarter.Value(GetType());

        /// <summary>
        /// 启动完成
        /// </summary>
        void OnPostStart();
    }

}
