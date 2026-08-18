using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PAV_PF_KendallBonilla_RandyArtavia
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Usuarios", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GetPrecioLibro",
                url: "Ventas/GetPrecioLibro/{id}",
                defaults: new { controller = "Ventas", action = "GetPrecioLibro", id = UrlParameter.Optional }
            );
        }
    }
}
