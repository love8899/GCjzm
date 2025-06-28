using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Validators.JobOrder;

namespace Wfm.Web.Models.JobOrder
{
    [Validator(typeof(JobOrderEmailAFriendValidator))]
    public partial class JobOrderEmailAFriendModel : BaseWfmModel
    {
        public int JobOrderId { get; set; }

        public string JobOrderName { get; set; }

        public string JobOrderSeName { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.EmailAFriend.Fields.FriendEmail")]
        public string FriendEmail { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.EmailAFriend.Fields.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.EmailAFriend.Fields.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}