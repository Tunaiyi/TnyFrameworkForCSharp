using System.Reflection;

namespace TnyFramework.Common.FastInvoke.ActionInvoke
{

    public interface IFastActionFactory
    {
        IFastInvoker CreateInvoker(FieldInfo field);

        IFastInvoker CreateInvoker(PropertyInfo property);

        IFastInvoker CreateInvoker(MethodInfo method);

        IFastInvoker CreateInvoker(ConstructorInfo constructor);
    }

}
