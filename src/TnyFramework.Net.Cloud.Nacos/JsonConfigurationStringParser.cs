// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Nacos.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TnyFramework.Net.Cloud.Nacos
{

    public class JsonConfigurationStringParser : INacosConfigurationParser
    {
        internal static JsonConfigurationStringParser instance = new JsonConfigurationStringParser();

        private readonly IDictionary<string, string> data =
            new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly Stack<string> context = new Stack<string>();
        private string currentPath;
        private JsonTextReader reader;

        private JsonConfigurationStringParser()
        {
        }

        public IDictionary<string, string> Parse(string input) => new JsonConfigurationStringParser().ParseString(input);

        private IDictionary<string, string> ParseString(string input)
        {
            data.Clear();
            if (string.IsNullOrEmpty(input))
            {
                input = "{}";
            }
            var jsonTextReader = new JsonTextReader(new StringReader(input));
            jsonTextReader.DateParseHandling = DateParseHandling.None;
            reader = jsonTextReader;
            VisitJObject(JObject.Load(reader));
            return data;
        }

        private void VisitJObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                EnterContext(property.Name);
                VisitProperty(property);
                ExitContext();
            }
        }

        private void VisitProperty(JProperty property) => VisitToken(property.Value);

        private void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;
                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                    VisitPrimitive(token.Value<JValue>());
                    break;
                default:
                    throw new FormatException(string.Format("Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.",
                        reader.TokenType, reader.Path, reader.LineNumber,
                        reader.LinePosition));
            }
        }

        private void VisitArray(JArray array)
        {
            for (var index = 0; index < array.Count; ++index)
            {
                EnterContext(index.ToString());
                VisitToken(array[index]);
                ExitContext();
            }
        }

        private void VisitPrimitive(JValue data)
        {
            var currentPath = this.currentPath;
            if (this.data.ContainsKey(currentPath))
                throw new FormatException("A duplicate key '" + currentPath + "' was found.");
            this.data[currentPath] = data.ToString(CultureInfo.InvariantCulture);
        }

        private void EnterContext(string context)
        {
            this.context.Push(context);
            currentPath = ConfigurationPath.Combine(this.context.Reverse());
        }

        private void ExitContext()
        {
            context.Pop();
            currentPath = ConfigurationPath.Combine(context.Reverse());
        }
    }

}
