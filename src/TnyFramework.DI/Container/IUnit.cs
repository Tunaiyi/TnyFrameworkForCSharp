using System;

namespace TnyFramework.DI.Container
{

    public interface IUnit<out TInstance>
    {
        /// <summary>
        /// 实例名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取 Unit 实体
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        TInstance Value(IServiceProvider provider);
    }

}
