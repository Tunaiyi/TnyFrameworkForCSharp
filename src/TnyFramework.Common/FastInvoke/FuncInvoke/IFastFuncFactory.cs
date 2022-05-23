using System.Reflection;

namespace TnyFramework.Common.FastInvoke.FuncInvoke
{

    public interface IFastFuncFactory
    {
        IFastInvoker CreateInvoker(FieldInfo field);

        IFastInvoker CreateInvoker(PropertyInfo property);

        IFastInvoker CreateInvoker(MethodInfo method);

        IFastInvoker CreateInvoker(ConstructorInfo constructor);
    }

}
