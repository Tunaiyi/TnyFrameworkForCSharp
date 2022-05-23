using System.Text;

namespace TnyFramework.Namespace
{

    public static class NamespacePathNames
    {
        public const string NAMESPACE_DELIMITER = "/";

        public static string NodePath(string parent, params object[] nodes)
        {
            var endDelimiter = parent.EndsWith(NAMESPACE_DELIMITER);
            var startsWith = parent.StartsWith(NAMESPACE_DELIMITER);
            var builder = new StringBuilder();
            if (!startsWith)
            {
                builder.Append(NAMESPACE_DELIMITER);
            }
            builder.Append(parent);
            if (endDelimiter)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            foreach (var node in nodes)
            {
                builder.Append(NAMESPACE_DELIMITER).Append(node);
            }
            return builder.ToString();
        }

        public static string DirPath(string parent, params object[] nodes)
        {
            bool endDelimiter = parent.EndsWith(NAMESPACE_DELIMITER);
            bool startsWith = parent.StartsWith(NAMESPACE_DELIMITER);
            StringBuilder builder = new StringBuilder();
            if (!startsWith)
            {
                builder.Append(NAMESPACE_DELIMITER);
            }
            builder.Append(parent);
            if (!endDelimiter)
            {
                builder.Append(NAMESPACE_DELIMITER);
            }
            foreach (var node in nodes)
            {
                builder.Append(node).Append(NAMESPACE_DELIMITER);
            }
            return builder.ToString();
        }
    }

}
