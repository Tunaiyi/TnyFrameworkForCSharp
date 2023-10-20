// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Namespace
{

    public class NamespacePath
    {
        private const string DELIMITER = NamespacePathNames.NAMESPACE_DELIMITER;

        private const string EMPTY_NODE = "";

        private static readonly NamespacePath ROOT = new();

        public string Node { get; }

        public string Pathname { get; }

        public NamespacePath? Parent { get; }

        public string ParentPathname => Parent?.Pathname!;

        public static NamespacePath Root()
        {
            return ROOT;
        }

        public static NamespacePath Path(NamespacePath? parent, string node)
        {
            if (parent == null)
            {
                return node.IsBlank() ? ROOT : new NamespacePath(null, node);
            }
            return node.IsBlank() ? parent : new NamespacePath(parent, node);
        }

        private NamespacePath()
        {
            Pathname = DELIMITER;
            Node = EMPTY_NODE;
            Parent = null;
        }

        private NamespacePath(NamespacePath? parent, string node)
        {
            Node = node;
            Parent = parent;
            Pathname = parent == null ? NamespacePathNames.DirPath(node) : NamespacePathNames.DirPath(parent.Pathname, node);
        }

        /// <summary>
        /// 连接节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>路径</returns>
        public NamespacePath Contact(object node)
        {
            var nodeValue = node.ToString();
            if (nodeValue == null)
            {
                throw new NullReferenceException();
            }
            return Path(this, nodeValue);
        }

        /// <summary>
        /// 是否是根路径
        /// </summary>
        /// <returns>是否是根路径</returns>
        public bool IsRoot()
        {
            return Parent == null;
        }
    }

}
