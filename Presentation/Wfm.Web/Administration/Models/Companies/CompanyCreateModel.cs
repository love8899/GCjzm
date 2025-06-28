using System;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Web.Framework;

namespace Wfm.Admin.Models.Companies
{
    public class CompanyBasicInformation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyGuid { get; set; }
        public int CompanyId { get; set; }
 
        public string CompanyName { get; set; }
        public string WebSite { get; set; }
        public string KeyTechnology { get; set; }
        [WfmResourceDisplayName("Common.Status")]
        public int CompanyStatusId { get; set; }
        public int InvoiceIntervalId { get; set; }
        public string Note { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
    }
}