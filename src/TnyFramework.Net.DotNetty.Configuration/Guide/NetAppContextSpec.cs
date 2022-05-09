using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public class NetAppContextSpec : UnitSpec<NetAppContext, INetUnitContext>, INetAppContextSpec
    {
        private readonly NetAppContext context = new NetAppContext();


        public NetAppContextSpec(string unitName = "") : base(unitName)
        {
            Default(_ => context);
        }


        public NetAppContextSpec ServerId(int value)
        {
            context.ServerId = value;
            return this;

        }


        public NetAppContextSpec AppName(string value)
        {
            context.Name = value;
            return this;
        }


        public NetAppContextSpec AppType(string value)
        {
            context.AppType = value;
            return this;
        }


        public NetAppContextSpec ScopeType(string value)
        {
            context.ScopeType = value;
            return this;
        }


        public NetAppContextSpec Locale(string value)
        {
            context.Locale = value;
            return this;
        }
    }
}
