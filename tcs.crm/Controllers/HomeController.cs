using System.Web.Mvc;
using tcs.adapter.Sql;

namespace tcs.crm.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [Permissions("Home.Index")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MainMenu()
        {
            return PartialView(AccountInfo);
        }

        public ActionResult Notify()
        {
            if (AccountInfo == null)
                return null;

            var notRead = 0;
            var lstNotify = NotifyDb.Instance.GetNew(AccountInfo.TitleId, 10, ref notRead);
            ViewBag.NotRead = notRead;
            return PartialView(lstNotify);
        }

        [HttpPost]
        public JsonResult UpdateNotify(string id)
        {
            var result = new { Code = 1, Msg = "Fail" };
            if (!string.IsNullOrEmpty(id))
            {
                NotifyDb.Instance.UpdateRead(id);
                result = new { Code = 0, Msg = "Success" };
            }
            return Json(result);
        }
    }
}