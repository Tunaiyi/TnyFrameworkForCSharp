#region

using System;

#endregion

namespace TnyFramework.DI.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ServiceAttribute : ComponentAttribute
    {
        public ServiceAttribute(string name = "") : base(name)
        {
        }
    }

}
