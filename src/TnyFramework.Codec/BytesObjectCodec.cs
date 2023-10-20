// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.IdentityModel.Tokens;

namespace TnyFramework.Codec
{

    public abstract class BytesObjectCodec<T> : ObjectCodec<T>
    {
        public override string Format(T? value)
        {
            if (value == null)
            {
                return null!;
            }
            var bytes = Encode(value);
            return Base64UrlEncoder.Encode(bytes);
        }

        public override T? Parse(string? data)
        {
            if (data == null)
            {
                return default!;
            }

            var bytes = Base64UrlEncoder.DecodeBytes(data);
            return Decode(bytes);
        }
    }

}
