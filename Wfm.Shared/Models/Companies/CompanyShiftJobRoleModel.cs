using FluentValidation.Attributes;
using Wfm.Shared.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
namespace Wfm.Shared.Models.Companies
{
    [Validator(typeof(CompanyShiftJobRoleValidator))]
    public class CompanyShiftJobRoleModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Shift")]
        public int CompanyShiftId { get; set; }
        [WfmResourceDisplayName("Web.EmployeeJobRole.CompanyJobRole")]
        public int CompanyJobRoleId { get; set; }
        [WfmResourceDisplayName("Web.CompanyShiftJobRole.Fields.MandantoryRequiredCount")]
        public int MandantoryRequiredCount { get; set; }
        [WfmResourceDisplayName("Web.CompanyShiftJobRole.Fields.ContingencyRequiredCount")]
        public int ContingencyRequiredCount { get; set; }
        [WfmResourceDisplayName("Web.EmployeeJobRole.CompanyJobRole")]
        public CompanyJobRoleDropdownModel CompanyJobRole { get; set; }
        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Supervisor")]
        public AccountDropdownModel Supervisor { get; set; }
        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Supervisor")]
        public int? SupervisorId { get; set; }
    }
    [Validator(typeof(CompanyShiftJobRoleGridValidator))]
    public class CompanyShiftJobRoleGridModel
    {
        [WfmResourceDisplayName("Common.Shift")]
        public int CompanyShiftId { get; set; }
        public int Id { get; set; }
        [WfmResourceDisplayName("Web.CompanyShiftJobRole.Fields.MandantoryRequiredCount")]
        public int MandantoryRequiredCount { get; set; }
        [WfmResourceDisplayName("Web.CompanyShiftJobRole.Fields.ContingencyRequiredCount")]
        public int ContingencyRequiredCount { get; set; }
        [WfmResourceDisplayName("Web.EmployeeJobRole.CompanyJobRole")]
        public CompanyJobRoleDropdownModel CompanyJobRole { get; set; }
        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Supervisor")]
        public AccountDropdownModel Supervisor { get; set; }
    }

    public class CompanyJobRoleDropdownModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AccountDropdownModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
