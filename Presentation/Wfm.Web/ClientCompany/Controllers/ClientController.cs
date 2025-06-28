using System.Web.Mvc;
using Wfm.Client.Extensions;

namespace Wfm.Client.Controllers
{
    public class ClientController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("SignIn", "Account");
        }
    }
}
