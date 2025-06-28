using System.Web.Mvc;

namespace Wfm.Web.Controllers
{
    public class AssociateController : Controller
    {
        //
        // GET: /Associate/

        public ActionResult Index()
        {
            return RedirectToAction("Associates");
        }

        public ActionResult Associates()
        {
            return View();
        }

        public ActionResult EmploymentGuidelines()
        {
            return View();
        }

        public ActionResult SafetyInformation()
        {
            return View();
        }
    }
}
