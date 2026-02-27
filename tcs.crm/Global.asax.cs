using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using tcs.adapter.Helper;
using tcs.lib;
using WebMatrix.WebData;

namespace tcs.crm
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("AccountConnectionString", "webpages_Users", "Id", "UserName", autoCreateTables: false);
            }

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

            Cacher.InitCache();
            CacheAllUserInRoles();
            AccountHelper.InitFuntionPermission();
        }

        /// <summary>
        /// Tất cả mọi kiểm tra phân quyền đều lấy từ cache này
        /// </summary>
        protected void CacheAllUserInRoles()
        {
            var dic = new Dictionary<string, string[]>();

            var allRoles = Roles.GetAllRoles();
            foreach (var role in allRoles)
            {
                var userInRole = Roles.GetUsersInRole(role);
                dic.Add(role, userInRole);
            }

            AccountHelper.CacheAllUserInRoles(dic);
        }
    }
}
