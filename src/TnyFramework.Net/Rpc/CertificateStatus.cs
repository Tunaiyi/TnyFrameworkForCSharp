using System;
namespace TnyFramework.Net.Rpc
{
    [Flags]
    public enum CertificateStatus
    {
        /**
		 * 无效的
		 */
        Invalid = 0,

        /**
		 * 未认证
		 */
        Unauthenticated = 1 << 1,

        /**
		 * 已认证
		 */
        Authenticated = (2 << 1) | 1,

        /**
		 * 续约认证
		 */
        Renew = (3 << 1) | 1,
    }

    public static class CertificateStatusExtensions
    {
        public static bool IsAuthenticated(this CertificateStatus status) => ((int)status & 1) == 1;
    }
}
