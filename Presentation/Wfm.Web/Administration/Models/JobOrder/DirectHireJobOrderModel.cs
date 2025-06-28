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
    [Validator(typeof(DirectHireJobOrderValidator))]
    public class DirectHireJobOrderModel : BaseWfmEntityModel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobOrderGuid { get; set; }
        [WfmResourceDisplayName("Common.Company")] 
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public Guid CompanyGuid { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int? CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobDescription")]
        [AllowHtml]
        public string JobDescription { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.CompanyJobNumber")]
        public string CompanyJobNumber { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.HiringDurationExpiredDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? HiringDurationExpiredDate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.SalaryMax")]
        public decimal? SalaryMax { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.SalaryMin")]
        public decimal? SalaryMin { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Salary")]
        public string Salary { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int JobOrderStatusId { get; set; } 

        [WfmResourceDisplayName("Common.Status")]
        public string Status { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        [WfmResourceDisplayName("Common.Recruiter")]
        public int? RecruiterId { get; set; }
        [WfmResourceDisplayName("Common.Owner")]
        public int? OwnerId { get; set; }
        [WfmResourceDisplayName("Common.FranchiseId")]
        public int? FranchiseId { get; set; }
        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }
        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }
     
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

        [WfmResourceDisplayName("Common.IsPublished")]
        public bool IsPublished { get; set; }

       [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.JobOrderCategoryId")]
        public int JobOrderCategoryId { get; set; }

       [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.FeeMin")]
       public decimal? FeeMin { get; set; }
       [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.FeeMax")]
       public decimal? FeeMax { get; set; }
       [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.FeePercent")]
       public decimal? FeePercent { get; set; }
       [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.FeeAmount")]
       public decimal? FeeAmount { get; set; }

       [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.FeeType")]
       public int? FeeTypeId { get; set; }

       [WfmResourceDisplayName("Admin.JobOrder.DirectHireJobOrder.Fields.FeeType")]
       public string FeeTypeName { get; set; } 
       public string SeName { get; set; }

       [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobOrderTypeId")]
       public int? JobOrderTypeId { get; set; }

       public Guid? FranchiseGuid { get; set; }

       public virtual JobOrderCategoryModel JobOrderCategoryModel { get; set; }
       public virtual JobOrderTypeModel JobOrderTypeModel { get; set; }

        
    }
}