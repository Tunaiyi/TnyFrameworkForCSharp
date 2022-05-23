namespace TnyFramework.Common.FastInvoke.ActionInvoke
{

    public abstract class BaseFastAction<TAction> : IFastInvoker
    {
        internal TAction Action { get; set; }

        public object Invoke(object invoker, params object[] parameters)
        {
            DoInvoke(invoker, parameters);
            return default;
        }

        protected abstract void DoInvoke(object invoker, params object[] parameters);
    }

}
