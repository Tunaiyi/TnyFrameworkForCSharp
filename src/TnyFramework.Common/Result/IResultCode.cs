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



        private static IResultCode Of(int id, string message, ResultLevel level = ResultLevel.General, Action<ResultCode> builder = null)
        {
            return E(id, it => {
                it.Message = message;
                it.Level = level;
                builder?.Invoke(it);
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
            return E(id, new T {
                    Message = message,
                    Level = level
                }, it => { builder?.Invoke((T) it); });
        }


        public new static IReadOnlyCollection<ResultCode> GetValues()
        {
            CheckAndUpdateNames(typeof(T));
            return ENUMS;
        }
        
        public new static ResultCode ForId(int id)
        {
            CheckAndUpdateNames(typeof(T));
            if (!ID_ENUM_MAP.TryGetValue(id, out var obj))
                throw new ArgumentException("枚举ID不存在 -> " + id.ToString());
            return obj;
        }


        public new static ResultCode ForName(string name)
        {
            CheckAndUpdateNames(typeof(T));
            if (!NAME_ENUM_MAP.TryGetValue(name, out var obj))
                throw new ArgumentException("枚举名称不存在 -> " + name);
            return obj;
        }
    }

}
