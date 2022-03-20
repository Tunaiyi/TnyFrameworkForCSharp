using System;
namespace TnyFramework.Common.Invoke
{
    public abstract class BaseFastAction<TAction> : IFastInvoker
    {
        protected readonly TAction func;


        protected BaseFastAction(TAction func)
        {
            this.func = func;
        }


        public object Invoke(object invoker, params object[] parameters)
        {
            DoInvoke(invoker, parameters);
            return default;
        }


        protected abstract void DoInvoke(object invoker, params object[] parameters);
    }

    public class FastAction<TInvoker> : BaseFastAction<Action<TInvoker>>
    {
        public FastAction(Action<TInvoker> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker);
        }
    }

    public class FastAction<TInvoker, TP1>
        : BaseFastAction<Action<TInvoker, TP1>>
    {
        public FastAction(Action<TInvoker, TP1> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker, (TP1)parameters[0]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2>
        : BaseFastAction<Action<TInvoker, TP1, TP2>>
    {
        public FastAction(Action<TInvoker, TP1, TP2> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1]);
        }
    }


    public class FastAction<TInvoker, TP1, TP2, TP3>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2]);
        }
    }


    public class FastAction<TInvoker, TP1, TP2, TP3, TP4>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7]);
        }
    }


    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7],
                (TP9)parameters[8]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7],
                (TP9)parameters[8],
                (TP10)parameters[9]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7],
                (TP9)parameters[8],
                (TP10)parameters[9],
                (TP11)parameters[10]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7],
                (TP9)parameters[8],
                (TP10)parameters[9],
                (TP11)parameters[10],
                (TP12)parameters[11]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7],
                (TP9)parameters[8],
                (TP10)parameters[9],
                (TP11)parameters[10],
                (TP12)parameters[11],
                (TP13)parameters[12]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7],
                (TP9)parameters[8],
                (TP10)parameters[9],
                (TP11)parameters[10],
                (TP12)parameters[11],
                (TP13)parameters[12],
                (TP14)parameters[13]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>>
    {
        public FastAction(Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15> func) : base(func)
        {
        }


        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            func.Invoke((TInvoker)invoker,
                (TP1)parameters[0],
                (TP2)parameters[1],
                (TP3)parameters[2],
                (TP4)parameters[3],
                (TP5)parameters[4],
                (TP6)parameters[5],
                (TP7)parameters[6],
                (TP8)parameters[7],
                (TP9)parameters[8],
                (TP10)parameters[9],
                (TP11)parameters[10],
                (TP12)parameters[11],
                (TP13)parameters[12],
                (TP14)parameters[13],
                (TP15)parameters[14]);
        }
    }
}
