using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Wfm.Admin.Validators.JobOrder;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.JobOrder
{
   
    public class DirectHireJobOrderListModel : BaseWfmEntityModel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobOrderGuid { get; set; } 
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public Guid CompanyGuid { get; set; }
      
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }     

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.CompanyJobNumber")]
        public string CompanyJobNumber { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.HiringDurationExpiredDate")]
        public DateTime? HiringDurationExpiredDate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.SalaryMax")]
        public decimal SalaryMax { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.SalaryMin")]
        public decimal SalaryMin { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Salary")]
        public string Salary { get; set; }    

        [WfmResourceDisplayName("Common.Status")]
        public string Status { get; set; }
    
     
        [WfmResourceDisplayName("Common.Recruiter")]
        public string RecruiterName { get; set; }
        [WfmResourceDisplayName("Common.Owner")]
        public string OwnerName { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string CompanyLocationName { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string CompanyDepartmentName { get; set; }

        [WfmResourceDisplayName("Common.FranchiseName")]
        public string FranchiseName { get; set; }

        [WfmResourceDisplayName("Common.FranchiseName")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.IsPublished")]
        public bool IsPublished { get; set; }

       [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.JobOrderCategoryId")]
        public string JobOrderCategory { get; set; }  

       [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.FeeType")]
       public string FeeType { get; set; }
         
      
    }
}