namespace TnyFramework.Common.Result
{
    public class DefaultResultCode : BaseResultCode<DefaultResultCode>
    {
        /// <summary>
        /// 重构结果码
        /// </summary>
        public static readonly IResultCode SUCCESS = Of(ResultConstants.SUCCESS_CODE, "Success");

        /// <summary>
        /// 失败结果码
        /// </summary>
        public static readonly IResultCode FAILURE = Of(ResultConstants.FAILURE_CODE, "Failure");
    }
}
