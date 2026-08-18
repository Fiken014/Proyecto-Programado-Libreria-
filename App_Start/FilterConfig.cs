using System.Web;
using System.Web.Mvc;

namespace PAV_PF_KendallBonilla_RandyArtavia
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
