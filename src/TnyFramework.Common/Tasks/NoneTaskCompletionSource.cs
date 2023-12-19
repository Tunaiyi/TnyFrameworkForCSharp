// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Common.Tasks
{

    public interface ITaskCompletionSource
    {
        Task Task { get; }

        void SetCanceled();

        // void SetCanceled(CancellationToken cancellationToken);

        void SetException(IEnumerable<Exception> exceptions);

        void SetException(Exception exception);

        void SetResult();

        bool TrySetCanceled();

        bool TrySetCanceled(CancellationToken cancellationToken);

        bool TrySetException(IEnumerable<Exception> exceptions);

        bool TrySetException(Exception exception);

        bool TrySetResult();
    }

    public class NoneTaskCompletionSource
#if NETSTANDARD2_1 || NETFRAMEWORK
        : TaskCompletionSource<object>, ITaskCompletionSource
    {
        public new Task Task => base.Task;

        public void SetResult()
        {
            SetResult(null!);
        }

        public bool TrySetResult()
        {
            return TrySetResult(null!);
        }
#else
        : TaskCompletionSource, ITaskCompletionSource
    {
        public NoneTaskCompletionSource()
        {
        }

        public NoneTaskCompletionSource(object? state) : base(state)
        {
        }

        public NoneTaskCompletionSource(object? state, TaskCreationOptions creationOptions) : base(state, creationOptions)
        {
        }

        public NoneTaskCompletionSource(TaskCreationOptions creationOptions) : base(creationOptions)
        {
        }
#endif
    }

}
