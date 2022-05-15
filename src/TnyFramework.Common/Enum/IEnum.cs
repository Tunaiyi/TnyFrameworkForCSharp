namespace TnyFramework.Common.Enum
{

    /// <summary>
    /// 枚举
    /// </summary>
    public interface IEnum
    {
        /// <summary>
        /// 枚举ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        string Name { get; }
    }

}
