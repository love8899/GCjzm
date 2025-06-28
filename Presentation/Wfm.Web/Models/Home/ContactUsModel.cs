using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Validators.Home;
using Wfm.Web.Framework;

namespace Wfm.Web.Models.Home
{
    [Validator(typeof(ContactUsValidator))]
    public class ContactUsModel
    {
        [WfmResourceDisplayName("Common.Subject")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string Subject { get; set; }


        [WfmResourceDisplayName("Web.Home.ContactUs.Fields.ContactName")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string ContactName { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string Company { get; set; }

        [WfmResourceDisplayName("Web.Home.ContactUs.Fields.Phone")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string Phone { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Web.Home.ContactUs.Fields.Message")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]\r\n:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string Message { get; set; }

        [WfmResourceDisplayName("Web.Home.ContactUs.Fields.RespondBy")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string RespondBy { get; set; }

        [WfmResourceDisplayName("Web.Home.ContactUs.Fields.BestTimeToRespond")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string BestTimeToRespond { get; set; }

       



        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

    }

}