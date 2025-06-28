using FluentValidation.Attributes;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Shared.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Companies
{
    [Validator(typeof(CompanyJobRoleValidator))]
    public class CompanyJobRoleModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Position")]
        public int PositionId { get; set; }
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        public int LocationId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }
        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }
        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }
        [WfmResourceDisplayName("Web.JobRole.RequiredSkills")]
        public string RequiredSkillNames { get; set; }
        [WfmResourceDisplayName("Web.JobRole.RequiredSkills")]
        public string[] RequiredSkillIds { get; set; }
        [WfmResourceDisplayName("Web.JobRole.StandardCostHourlyRate")]
        public decimal? StandardCostHourlyRate { get; set; }
        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Supervisor")]

        public IEnumerable<SelectListItem> LocationList
        {
            get; set;
        }
        public IEnumerable<SelectListItem> SkillList
        {
            get; set;
        }
        public IEnumerable<SelectListItem> SelectedSkillList
        {
            get; set;
        }
    }
}
