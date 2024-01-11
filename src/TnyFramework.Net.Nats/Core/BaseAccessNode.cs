// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Nats.Core
{

    public abstract class BaseAccessNode : INatsAccessNode
    {
        public string NodeKey { get; private set; } = "";

        public string AccessType { get; private set; } = "";

        public string Service => AccessType;

        public long NodeId { get; private set; }

        public long AccessId { get; private set; }

        protected virtual void Init(string nodeKey)
        {
            var worlds = nodeKey.Split("#");
            DoInit(nodeKey, worlds);
        }

        protected void DoInit(string nodeKey, string[] worlds)
        {
            AccessType = worlds[0];
            NodeId = int.Parse(worlds[1]);
            AccessId = long.Parse(worlds[2]);
            NodeKey = nodeKey;
        }

        protected void Init(string accessType, long nodeId, long accessId)
        {
            AccessType = accessType;
            NodeId = nodeId;
            AccessId = accessId;
            NodeKey = $"{AccessType}#{NodeId}#{AccessId}";
        }

        protected void Init(INatsAccessNode access)
        {
            Init(access.AccessType, access.NodeId, access.AccessId);
        }

        protected virtual void Clear()
        {
            AccessType = "";
            NodeId = 0;
            AccessId = 0;
        }

    }

}
