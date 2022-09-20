// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Codec
{

    public class MimeTypes : MimeType<MimeTypes>
    {
        public const string JSON_TYPE = "json";

        public const string PROTOBUF_TYPE = "protobuf";

        public const string TYPE_PROTOBUF_TYPE = "type-protobuf";

        public static readonly MimeType JSON = Of(100, JSON_TYPE);

        public static readonly MimeType PROTOBUF = Of(200, PROTOBUF_TYPE);

        public static readonly MimeType TYPE_PROTOBUF = Of(300, TYPE_PROTOBUF_TYPE);
    }

}
