// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Namespace
{

    public class NameNode<TValue>
    {
        public string Name { get; }

        public long Id { get; }

        public TValue Value { get; }

        public long Version { get; }

        public long Revision { get; }

        public bool Delete { get; }

        public NameNode(string name, long id, TValue value, long version, long revision, bool delete)
        {
            Name = name;
            Id = id;
            Value = value;
            Version = version;
            Revision = revision;
            Delete = delete;
        }

        public override string ToString()
        {
            return
                $"{nameof(Name)}: {Name}, {nameof(Id)}: {Id}, {nameof(Value)}: {Value}, {nameof(Version)}: {Version}, {nameof(Revision)}: {Revision}, {nameof(Delete)}: {Delete}";
        }
    }

}
