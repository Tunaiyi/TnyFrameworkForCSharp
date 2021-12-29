using System;
using TnyFramework.Common.Enum;
namespace TnyFramework.Common.Result
{
    public interface IResultCode : IEnum
    {
        /// <summary>
        /// 结果码值
        /// </summary>
        int Code { get; }

        /// <summary>
        /// 错误描述 
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 结果等级
        /// </summary>
        ResultLevel Level { get; }
    }

    public abstract class BaseResultCode<T> : BaseEnum<T>, IResultCode where T : BaseResultCode<T>, new()
    {
        protected static IResultCode Of(int id, string message, ResultLevel level = ResultLevel.General, Action<T> builder = null)
        {
            return E(id, it => {
                it.Message = message;
                it.Level = level;
            });
        }


        /// <summary>
        /// 结果码值
        /// </summary>
        public int Code => Id;

        /// <summary>
        /// 错误描述 
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// 结果等级
        /// </summary>
        public ResultLevel Level { get; protected set; } = ResultLevel.General;
    }
}
