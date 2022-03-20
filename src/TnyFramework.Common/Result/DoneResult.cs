namespace TnyFramework.Common.Result
{
    public class DoneResult<TValue> : IDoneResult<TValue>
    {
        public IResultCode Code { get; }

        public int CodeValue => Code.Value;
        public TValue Value { get; }
        public string Message { get; }


        public DoneResult(IResultCode code, TValue value, string message)
        {
            Message = message;
            Code = code;
            Value = value;
        }


        public bool IsSuccess()
        {
            return DoneResults.IsSuccess(Code.Value);
        }


        public bool IsPresent()
        {
            return Value != null;
        }


        public object GetValue()
        {
            return Value;
        }
    }
}
