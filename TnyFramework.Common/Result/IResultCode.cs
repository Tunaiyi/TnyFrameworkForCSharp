using System;
using System.Collections.Generic;
using TnyFramework.Common.Enum;
namespace TnyFramework.Common.Result
{
    public interface IResultCode : IEnum
    {
        /// <summary>
        /// 结果码值
        /// </summary>
        int Value { get; }

        /// <summary>
        /// 错误描述 
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 结果等级
        /// </summary>
        ResultLevel Level { get; }


        bool IsSuccess();

        bool IsFailure();
    }

    public class ResultCode : BaseEnum<ResultCode>, IResultCode
    {
        public const int SUCCESS_CODE = 100;
        public const int FAILURE_CODE = 101;


        /// <summary>
        /// 重构结果码
        /// </summary>
        public static readonly IResultCode SUCCESS = Of(ResultConstants.SUCCESS_CODE, "Success");

        /// <summary>
        /// 失败结果码
        /// </summary>
        public static readonly IResultCode FAILURE = Of(ResultConstants.FAILURE_CODE, "Failure");



        protected static IResultCode Of(int id, string message, ResultLevel level = ResultLevel.General, Action<ResultCode> builder = null)
        {
            return E(id, it => {
                it.Message = message;
                it.Level = level;
            });
        }


        /// <summary>
        /// 结果码值
        /// </summary>
        public int Value => Id;

        /// <summary>
        /// 错误描述 
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// 结果等级
        /// </summary>
        public ResultLevel Level { get; protected set; } = ResultLevel.General;


        public bool IsSuccess()
        {
            return Value == SUCCESS_CODE;
        }


        public bool IsFailure()
        {
            return !IsSuccess();
        }
    }

    public abstract class ResultCode<T> : ResultCode where T : ResultCode<T>, new()
    {
        protected static IResultCode Of(int id, string message, ResultLevel level = ResultLevel.General, Action<T> builder = null)
        {
            var item = new T();
            return E(id, item, it => { builder?.Invoke((T)it); });
        }


        public new static IReadOnlyCollection<ResultCode> GetValues()
        {
            CheckAndUpdateNames(typeof(T));
            return ENUMS;
        }
    }
}
