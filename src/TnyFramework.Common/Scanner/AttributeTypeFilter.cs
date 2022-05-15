namespace TnyFramework.Common.Scanner
{

    public class AttributeTypeFilter : TypesFilter<AttributeTypeFilter>
    {
        public AttributeTypeFilter() : base(TypeFilterExtensions.MatchAttributes)
        {
        }
    }

}
