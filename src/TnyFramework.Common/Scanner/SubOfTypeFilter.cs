namespace TnyFramework.Common.Scanner
{

    public class SubOfTypeFilter : TypesFilter<SubOfTypeFilter>
    {
        public SubOfTypeFilter() : base(TypeFilterExtensions.MatchSuper)
        {
        }
    }

}
