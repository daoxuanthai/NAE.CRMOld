using System.Web.Http.Cors;
using System.Web.Mvc;

namespace tcs.api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
