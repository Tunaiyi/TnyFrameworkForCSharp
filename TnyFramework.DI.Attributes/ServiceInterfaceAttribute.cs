using System;
namespace TnyFramework.DI.Attributes
{
    /// <summary>
    /// 自定注入特性
    /// 标记的接口代用注入时候会自动加载
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ServiceInterfaceAttribute : Attribute
    {
        public ServiceInterfaceAttribute()
        {
        }
    }
}
