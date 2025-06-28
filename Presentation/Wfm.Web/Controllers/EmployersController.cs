using System.Web.Mvc;

namespace Wfm.Web.Controllers
{
    public partial class EmployersController : BaseWfmController
    {
        // GET: /Employers/
       
        public ActionResult Index()
        {
            return RedirectToAction("Employers");
        }

        public ActionResult Employers()
        {
            return View();
        }

        public ActionResult Employers_StaffingSolutions()
        {
            return View();
        }

        public ActionResult Employers_StaffingSolutions_PermanentStaffing()
        {
            return View();
        }

        public ActionResult Employers_StaffingSolutions_TemporaryStaffing()
        {
            return View();
        }

        public ActionResult Employers_StaffingSolutions_MassRecruitmentServices()
        {
            return View();
        }

        public ActionResult Employers_StaffingSolutions_TemporaryForeignWorkerProgram()
        {
            return View();
        }

        public ActionResult Employers_SafetyAndPrevention()
        {
            return View();
        }

        public ActionResult Employers_Training()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_ManagementAndExecutive()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_SalesAndMarketing()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_FinancialAndAccounting()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_InformationTechnology()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_LifeSciences()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_Engineering()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_IndustrialAndManufacturing()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_CallCentreAndCustomerService()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_SupplyChainLogisticsAndWarehousing()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_AdministrativeAndOffice()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_SkilledTrades()
        {
            return View();
        }

        public ActionResult Employers_AreaSpecialties_Others()
        {
            return View();
        }

        public ActionResult Employers_Testimonials()
        {
            return View();
        }

        public ActionResult Employers_FindStaff()
        {
            return View();
        }



        //[HttpPost]
        //public ActionResult Employers_FindStaff(HomeRequestStaff model)
        //{

        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {

        //            if (ModelState.IsValid)
        //            {
        //                UserHelper _User = new UserHelper();
        //                string EmailTo = _User.GetSystemDefaultEmailAddress();

        //                string _Subject = "Request Staff";


        //                // Going to send email 
        //                StringBuilder sb = new StringBuilder();
        //                sb.Append("<html>");
        //                sb.Append("<title>" + _Subject + "</title>");
        //                sb.Append("<body>");
        //                sb.Append("<br /> ");
        //                sb.Append("Company Name :" + model.CompanyName1 + "<br />");
        //                sb.Append("Contact Name :" + model.ContactName + "<br />");
        //                sb.Append("Address      :" + model.Address + "<br />");
        //                sb.Append("City         :" + model.City + "<br />");
        //                sb.Append("Postal Code  :" + model.PostalCode + "<br />");
        //                sb.Append("Telephone    :" + model.Telephone + "<br />");
        //                sb.Append("Fax          :" + model.Fax + "<br />");
        //                sb.Append("Email Address:" + model.EmailAddress + "<br />");
        //                sb.Append("Job Description   :" + model.JobDescription + "<br />");
        //                sb.Append("Job Type         :" + model.JobType + "<br />");
        //                sb.Append("Number of people needed          :" + model.NumberOfPeople + "<br />");
        //                sb.Append("Start Date          :" + model.StartDate + "<br />");
        //                sb.Append("Finish date or duration         :" + model.FinishDateDuration + "<br />");
        //                sb.Append("Start time         :" + model.StartTime + "<br />");
        //                sb.Append("Finish time          :" + model.FinishTime + "<br />");
        //                sb.Append("Reason for staff request         :" + model.ReasonRequest + "<br />");
        //                sb.Append("Comment         :" + model.Comment + "<br />");
        //                sb.Append("</body>");
        //                sb.Append("</html>");

        //                EmailManager _EM = new EmailManager();
        //                string _RetMessage = _EM.SendEmail(model.EmailAddress, EmailTo, _Subject, sb.ToString(), true);

        //                // record to email history
        //                MessageHistory _EH = new MessageHistory();
        //                _EH.MailFrom = model.EmailAddress;
        //                _EH.MailTo = EmailTo;
        //                _EH.TextMessage = sb.ToString();
        //                _EH.UserId = ServerConfiguration.GetSystemDefaultID();
        //                _EH.Note = "Request Staff";
        //                _EH.CreatedOnUtc = System.DateTime.UtcNow;
        //                _EH.UpdatedOnUtc = System.DateTime.UtcNow;
        //                db.EmailHistories.Add(_EH);
        //                db.SaveChanges();

        //                ViewBag.Message = "Email Message has been sent, we will contact you shortly. Thank you for your request.";
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "Send email message with errors, please try again... or call us";
        //    }

        //    return View();
        //}



    }
}

