using System.Web.Mvc;
using System.Web.Routing;

namespace tcs.crm
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "hoi thao",
               "SeminarRegister/{id}",
                  new { controller = "SeminarRegister", action = "Index" },
                  new { id = @"^(\d)+$" }
               );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
