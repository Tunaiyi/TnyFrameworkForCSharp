namespace TnyFramework.Common.Invoke
{
    public interface IFastInvoker
    {
        object Invoke(object invoker, params object[] parameters);
    }

}
