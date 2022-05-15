namespace TnyFramework.Common.FastInvoke
{

    public interface IFastInvoker
    {
        object Invoke(object invoker, params object[] parameters);
    }

}
