using TnyFramework.Common.Result;
using TnyFramework.Net.Command;

namespace TnyFramework.Net.Dispatcher
{

    public interface IMessageCommandContext
    {
        string Name { get; }

        /**
		 * 设置CommandResult,并中断执行
		 *
		 * @param result 运行结果
		 */
        void Intercept(IRpcResult result);

        /**
		 * 设置结果码,并中断执行
		 *
		 * @param code 结果码
		 */
        void Intercept(IResultCode code);

        /**
         * 设置结果码,消息体,并中断执行
         */
        void Intercept(IResultCode code, object body);
    }

}
