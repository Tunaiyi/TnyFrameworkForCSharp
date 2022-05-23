using TnyFramework.Common.Extensions;

namespace TnyFramework.Namespace
{

    public class NamespacePath
    {
        private const string DELIMITER = NamespacePathNames.NAMESPACE_DELIMITER;

        private const string EMPTY_NODE = "";

        private static readonly NamespacePath ROOT = new NamespacePath();

        public string Node { get; }

        public string Pathname { get; }

        public NamespacePath Parent { get; }

        public string ParentPathname => Parent?.Pathname;

        public static NamespacePath Root()
        {
            return ROOT;
        }

        public static NamespacePath Path(NamespacePath parent, string node)
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

        private NamespacePath(NamespacePath parent, string node)
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
