namespace TnyFramework.Common.FastInvoke.FuncInvoke
{

    public abstract class BaseFastFunc<TFunc> : IFastInvoker
    {
        internal TFunc Func { get; set; }

        public abstract object Invoke(object invoker, params object[] parameters);
    }

}
