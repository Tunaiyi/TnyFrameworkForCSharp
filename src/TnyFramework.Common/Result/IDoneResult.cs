// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Result
{

    public interface IDoneResult : IDone
    {
        IResultCode Code { get; }

        int CodeValue { get; }
    }

    public interface IDoneResult<out TValue> : IDoneResult, IDone<TValue>
    {
    }

    public static class DoneResultExtensions
    {
        /// <summary>
        /// 如果失败则执行 Action<M> run
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="run">执行方法</param>
        /// <typeparam name="TM"></typeparam>
        public static void IfFailure<TM>(this IDoneResult<TM> done, Action<IResultCode, TM> run)
        {
            if (done.IsFailure())
            {
                run(done.Code, done.Value);
            }
        }

        /// <summary>
        /// 调用 consumer
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="run">运行</param>
        public static void Then<TM>(this IDoneResult<TM> done, Action<IResultCode, TM> run)
        {
            run(done.Code, done.Value);
        }

        /// <summary>
        /// 如果成功则转换值
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="mapper">转换</param>
        /// <typeparam name="TM"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDoneResult<T> MapOnSuccess<TM, T>(this IDoneResult<TM> done, Func<TM, T> mapper)
        {
            return done.IsSuccess() ? DoneResults.Success(mapper(done.Value)) : DoneResults.Failure<TM, T>(done);
        }

        public static IDoneResult<T> MapOnFailed<TM, T>(this IDoneResult<TM> done, Func<IResultCode, string, T> mapper)
        {
            return done.IsFailure() ? DoneResults.Map(done, mapper(done.Code, done.Message)) : DoneResults.Success<T>();
        }

        //    default <T> DoneResult<T> mapOnFailed(Function<DoneResult<M>, T> mapper) {
        //        if (this.isFailure()) {
        //            return DoneResults.map(this, mapper.apply(this));
        //        } else {
        //            return DoneResults.success();
        //        }
        //    }
        public static IDoneResult<T> MapIfPresent<TM, T>(this IDoneResult<TM> done, Func<TM, IResultCode, T> mapper)
        {
            var value = done.Value;
            if (value == null)
                return DoneResults.Map(done, default(T)!);
            var returnValue = mapper(value, done.Code);
            return DoneResults.Map(done, returnValue);
        }
    }

}
