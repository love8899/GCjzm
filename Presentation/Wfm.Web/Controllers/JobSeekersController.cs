using System.Web.Mvc;

namespace Wfm.Web.Controllers
{
    public class JobSeekersController : BaseWfmController
    {
        // GET: /JobSeekers/

        public ActionResult Index()
        {
            return RedirectToAction("JobSeekers");
        }

        public ActionResult JobSeekers()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_ManagementAndExecutive()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_SalesAndMarketing()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_FinancialAndAccounting()
        {

            return View();
        }

        public ActionResult JobSeekers_JobCategory_InformationTechnology()
        {

            return View();
        }

        public ActionResult JobSeekers_JobCategory_LifeSciences()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_Engineering()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_IndustrialAndManufacturing()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_CallCentreAndCustomerService()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_SupplyChainLogisticsAndWarehousing()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_AdministrativeAndOffice()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_SkilledTrades()
        {
            return View();
        }

        public ActionResult JobSeekers_JobCategory_Others()
        {
            return View();
        }

        public ActionResult JobSeekers_JobSearchResource()
        {
            return View();
        }

        public ActionResult JobSeekers_JobSearchResource_PreparingYourResume()
        {
            return View();
        }

        public ActionResult JobSeekers_JobSearchResource_WritingEffectiveJobSearchLetters()
        {
            return View();
        }

        public ActionResult JobSeekers_JobSearchResource_NetworkingEssentials()
        {
            return View();
        }

        public ActionResult JobSeekers_JobSearchResource_InterviewPreparation()
        {
            return View();
        }

        public ActionResult JobSeekers_TrainingAndDevelopment()
        {
            return View();
        }


        public ActionResult JobSeekers_Testimonials()
        {
            return View();
        }
    }
}
