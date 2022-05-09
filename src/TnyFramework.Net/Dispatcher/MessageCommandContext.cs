using System;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command;


namespace TnyFramework.Net.Dispatcher
{
    public class MessageCommandContext : IMessageCommandContext
    {
        public MessageCommandContext(MethodControllerHolder controller)
        {
            Controller = controller;
        }


        public MethodControllerHolder Controller { get; }

        public string Name => Controller.Name;

        public object Result { get; private set; }

        public Exception Cause { get; private set; }

        public bool Done { get; private set; }


        public void Complete(object result)
        {
            if (Done)
            {
                return;
            }
            if (result is Exception cause)
            {
                Cause = cause;
                Done = true;
            } else
            {
                Result = result;
                Done = true;
            }
        }


        public void Intercept(IRpcResult result)
        {

            Complete(result);
        }


        public void Intercept(IResultCode code)
        {
            Complete(code);
        }


        public void Intercept(IResultCode code, object body)
        {
            Complete(RpcResults.Result(code, body));
        }
    }
}
