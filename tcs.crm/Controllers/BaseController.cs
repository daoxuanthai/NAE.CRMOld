using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Web.Mvc;
using tcs.adapter.Helper;
using tcs.bo;
using tcs.lib;
using WebMatrix.WebData;

namespace tcs.crm.Controllers
{
    public class BaseController : Controller
    {
        public int CompanyId { get; set; }

        //public AccountInfo AccountInfo => Session["AccountInfo"] as AccountInfo;
        public AccountInfo AccountInfo
        {
            get
            {
                var info = Session["AccountInfo"] as AccountInfo;

                if (info != null)
                    return info;

                // Session chết nhưng cookie còn
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var userName = User.Identity.Name;
                    var userId = WebSecurity.GetUserId(userName);

                    info = AccountHelper.GetAccountInfo(userId, userName);
                    Session["AccountInfo"] = info;

                    return info;
                }

                return null;
            }
        }

        /// <summary>
        /// Render a view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model"></param>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public string RenderViewToString(string viewName, object model = null, ViewDataDictionary viewData = null)
        {
            // first find the ViewEngine for this view
            var viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);

            if (viewEngineResult == null)
            {
                if (!viewName.EndsWith(".cshtml"))
                    viewName = viewName + ".cshtml";
                viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);

                if (viewEngineResult == null)
                    throw new FileNotFoundException("View cannot be found.");
            }

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            ViewData.Model = model;
            if (viewData != null)
            {
                ViewData.Clear();
                foreach (var vd in viewData)
                {
                    ViewData.Add(vd.Key, vd.Value);
                }
            }

            string result;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(ControllerContext, view, ViewData, TempData, sw);
                view.Render(ctx, sw);
                viewEngineResult.ViewEngine.ReleaseView(ControllerContext, view);
                result = ConfigMgr.IsLiveEnv ? ViewHelper.MinifyHtml(sw.ToString()) : sw.ToString();
            }

            return result.Replace("cdn.tgdd.vn", ConfigMgr.ContentCdn);
        }

        /// <summary>
        /// Render a partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model"></param>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public string RenderPartialViewToString(string viewName, object model = null, ViewDataDictionary viewData = null)
        {
            try
            {
                // first find the ViewEngine for this view
                var viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);

                if (viewEngineResult == null)
                {
                    if (!viewName.EndsWith(".cshtml"))
                        viewName = viewName + ".cshtml";
                    viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);

                    if (viewEngineResult == null)
                        throw new FileNotFoundException("View cannot be found.");
                }

                // get the view and attach the model to view data
                var view = viewEngineResult.View;
                if (view == null)
                    throw new FileNotFoundException("View cannot get.");

                ViewData.Model = model;
                if (viewData != null)
                {
                    ViewData.Clear();
                    foreach (var vd in viewData)
                    {
                        ViewData.Add(vd.Key, vd.Value);
                    }
                }

                string result;

                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(ControllerContext, view, ViewData, TempData, sw);
                    view.Render(ctx, sw);
                    result = ConfigMgr.IsLiveEnv ? ViewHelper.MinifyHtml(sw.ToString()) : sw.ToString();
                }

                return result.Replace("cdn.tgdd.vn", ConfigMgr.ContentCdn);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Extension.JsonResult Error(string message = "")
        {
            return new Extension.JsonResult(HttpStatusCode.BadRequest, new
            {
                Message = message
            });
        }

        public Extension.JsonResult Success(string message = "", string html = "", int id = 0)
        {
            return new Extension.JsonResult(HttpStatusCode.OK, new
            {
                Id = id,
                Html = html,
                Message = message
            });
        }
    }

    /// <summary>
    /// Kiểm tra phân quyền xem user có được phân quyền sử dụng chức năng nào đó
    /// </summary>
    public class PermissionsAttribute : ActionFilterAttribute
    {
        private readonly string _required;

        public PermissionsAttribute(string required)
        {
            _required = required;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (System.Web.HttpContext.Current.Session["AccountInfo"] == null)
            if(!filterContext.HttpContext.Request.IsAuthenticated)
            {
                var currentUrl = filterContext.HttpContext.Request.RawUrl;
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("~/Account/Login");
                loginUrl += "?returnUrl=" + currentUrl;
                if(filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var result = new Extension.JsonResult(HttpStatusCode.Unauthorized, new
                    {
                        Message = "Phiên làm việc đã hết hạn vui lòng đăng nhập lại"
                    });

                    filterContext.Result = result;
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    filterContext.HttpContext.Response.Redirect(loginUrl, true);
                }
            }
            //else
            //{
            //    var info = System.Web.HttpContext.Current.Session["AccountInfo"] as AccountInfo;
            //    if (info == null || !AccountHelper.CheckPermission(info, _required))
            //    {
            //        throw new AuthenticationException("You do not have the necessary permission to perform this action");
            //    }
            //    // load lại thông tin user từ DB
            //    //var username = User.Identity.Name
            //}
        }
    }

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                return;

            var modelState = filterContext.Controller.ViewData.ModelState;
            if (!modelState.IsValid)
            {
                var errorModel =
                    from x in modelState.Keys
                    where modelState[x].Errors.Count > 0
                    select new
                    {
                        key = x,
                        msg = modelState[x].Errors[0].ErrorMessage
                    };

                var result = new Extension.JsonResult(HttpStatusCode.BadRequest, new
                {
                    Error = errorModel
                });

                filterContext.Result = result;
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            }
        }
    }
}