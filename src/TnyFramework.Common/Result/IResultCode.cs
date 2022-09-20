// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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

        public new static ResultCode ForId(int id)
        {
            return BaseEnum<ResultCode>.ForId(id);
        }

        public new static ResultCode ForName(string name)
        {
            return BaseEnum<ResultCode>.ForName(name);
        }
    }

    public abstract class ResultCode<T> : ResultCode where T : ResultCode<T>, new()
    {
        protected static T Of(int id, string message, ResultLevel level = ResultLevel.General, Action<T> builder = null)
        {
            return E(id, new T {
                Message = message,
                Level = level
            }, builder);
        }

        public new static void LoadAll() => LoadAll(typeof(T));

        public new static IReadOnlyCollection<ResultCode> GetValues()
        {
            LoadAll(typeof(T));
            return BaseEnum<ResultCode>.GetValues();
        }
    }

}
