using System.Web.Mvc;
using Wfm.Admin.Extensions;

namespace Wfm.Admin.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
          
            return RedirectToAction("SignIn","Account");
        }
    }
}

