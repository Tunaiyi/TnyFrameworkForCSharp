namespace TnyFramework.Coroutines.Async
{

    public class CoroutineState<T>
    {
        public T Result { get; }

        public bool Continue { get; }

        public static CoroutineState<T> GoOn()
        {
            return new CoroutineState<T>(default, true);
        }

        public static CoroutineState<T> Done(T result)
        {
            return new CoroutineState<T>(result, false);
        }

        private CoroutineState(T result, bool @continue)
        {
            Result = result;
            Continue = @continue;
        }
    }

}
