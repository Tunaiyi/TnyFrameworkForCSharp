// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.FastInvoke.ActionInvoke
{

    public class FastAction<TInvoker> : BaseFastAction<Action<TInvoker>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker);
        }
    }

    public class FastAction<TInvoker, TP1>
        : BaseFastAction<Action<TInvoker, TP1>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker, (TP1) parameters[0]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2>
        : BaseFastAction<Action<TInvoker, TP1, TP2>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
                (TP1) parameters[0],
                (TP2) parameters[1],
                (TP3) parameters[2],
                (TP4) parameters[3],
                (TP5) parameters[4],
                (TP6) parameters[5],
                (TP7) parameters[6]);
        }
    }

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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

    public class FastAction<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>
        : BaseFastAction<Action<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>>
    {
        protected override void DoInvoke(object invoker, params object[] parameters)
        {
            Action.Invoke((TInvoker) invoker,
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
