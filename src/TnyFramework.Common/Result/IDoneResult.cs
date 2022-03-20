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
        /// <typeparam name="M"></typeparam>
        public static void IfFailure<M>(this IDoneResult<M> done, Action<IResultCode, M> run)
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
        public static void Then<M>(this IDoneResult<M> done, Action<IResultCode, M> run)
        {
            run(done.Code, done.Value);
        }


        /// <summary>
        /// 如果成功则转换值
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="mapper">转换</param>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDoneResult<T> MapOnSuccess<M, T>(this IDoneResult<M> done, Func<M, T> mapper)
        {
            return done.IsSuccess() ? DoneResults.Success<T>(mapper(done.Value)) : DoneResults.Failure<M, T>(done);
        }


        public static IDoneResult<T> MapOnFailed<M, T>(this IDoneResult<M> done, Func<IResultCode, string, T> mapper)
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
        public static IDoneResult<T> MapIfPresent<M, T>(this IDoneResult<M> done, Func<M, IResultCode, T> mapper)
        {
            var value = done.Value;
            if (value == null)
                return DoneResults.Map(done, default(T));
            var returnValue = mapper(value, done.Code);
            return DoneResults.Map(done, returnValue);
        }
    }
}
