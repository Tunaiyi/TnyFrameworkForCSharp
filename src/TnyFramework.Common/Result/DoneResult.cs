// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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

        public bool IsFailure()
        {
            return !IsSuccess();
        }

        public bool IsPresent()
        {
            return Value != null;
        }

        public object GetValue()
        {
            return Value!;
        }
    }

}
