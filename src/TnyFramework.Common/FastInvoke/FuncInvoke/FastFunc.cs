using System;

namespace TnyFramework.Common.FastInvoke.FuncInvoke
{

    public class FastFunc<TInvoker, TResult> : BaseFastFunc<Func<TInvoker, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker);
        }
    }

    public class FastFunc<TInvoker, TP1, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker, (TP1) parameters[0]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7],
                (TP9) parameters[8]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7],
                (TP9) parameters[8],
                (TP10) parameters[9]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7],
                (TP9) parameters[8],
                (TP10) parameters[9],
                (TP11) parameters[10]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7],
                (TP9) parameters[8],
                (TP10) parameters[9],
                (TP11) parameters[10],
                (TP12) parameters[11]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7],
                (TP9) parameters[8],
                (TP10) parameters[9],
                (TP11) parameters[10],
                (TP12) parameters[11],
                (TP13) parameters[12]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7],
                (TP9) parameters[8],
                (TP10) parameters[9],
                (TP11) parameters[10],
                (TP12) parameters[11],
                (TP13) parameters[12],
                (TP14) parameters[13]);
        }
    }

    public class FastFunc<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>
        : BaseFastFunc<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>>
    {
        
        public override object Invoke(object invoker, params object[] parameters)
        {
            return Func.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6],
                (TP8) parameters[7],
                (TP9) parameters[8],
                (TP10) parameters[9],
                (TP11) parameters[10],
                (TP12) parameters[11],
                (TP13) parameters[12],
                (TP14) parameters[13],
                (TP15) parameters[14]);
        }
    }

}
