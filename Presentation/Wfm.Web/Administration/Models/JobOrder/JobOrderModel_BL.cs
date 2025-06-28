using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Services.JobOrders;
using Wfm.Admin.Extensions;
using Wfm.Services.Companies;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Policies;
using Wfm.Core.Domain.Policies;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Accounts;
using Wfm.Core;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Common;
using Wfm.Services.Common;

 

namespace Wfm.Admin.Models.JobOrder
{
    public class JobOrderModel_BL
    {
        #region Fields

        private const decimal JOB_ORDER_MINIMUM_DURATION_IN_HOURS = 1m;
        private const decimal JOB_ORDER_MAXIMUM_DURATION_IN_HOURS = 15m;

        private IWorkContext _workContext;
        private ILocalizationService _localizationService;
        private IJobOrderService _jobOrderService;
        private IJobOrderTestCategoryService _jobOrderTestCategoryService;
        private readonly IPositionService _positionService;
        private readonly ICompanyService _companyService;
        private IAccountService _accountService;

        #endregion

        #region Ctor

        public JobOrderModel_BL(
            IWorkContext workContext,
            ILocalizationService localizationService,
            IJobOrderService jobOrderService,
            IJobOrderTestCategoryService jobOrdertestCategoryService,
            IPositionService positionService,
            ICompanyService companyService,
            IAccountService accountService)
        {
            _workContext = workContext;
            _localizationService = localizationService;
            _jobOrderService = jobOrderService;
            _jobOrderTestCategoryService = jobOrdertestCategoryService;
            _positionService = positionService;
            _companyService = companyService;
            _accountService = accountService;
        }

        #endregion


        public JobOrderModel GetModelFromJobOrder(DateTime? refDate, Wfm.Core.Domain.JobOrders.JobOrder jobOrder, ICompanyDivisionService _companyDivisionService,
                                                ICompanyDepartmentService _companyDepartmentService, ISchedulePolicyService _schedulePolicyService,
                                                ICompanyContactService _companyContactService, ICompanyBillingService _companyBillingService,
                                                IAccountService _accountService)
        {
            JobOrderModel model = jobOrder.ToModel();

            CompanyLocation companyLocation = _companyDivisionService.GetCompanyLocationById(jobOrder.CompanyLocationId);
            CompanyDepartment companyDepartment = _companyDepartmentService.GetCompanyDepartmentById(jobOrder.CompanyDepartmentId);
            SchedulePolicy schedulePolicy = _schedulePolicyService.GetSchedulePolicyById(jobOrder.SchedulePolicyId);
            Position position = _positionService.GetPositionById(jobOrder.PositionId);
            if (position != null)
            {
                model.PositionName = position.Name;
            }
            if (companyLocation != null)
            {
                model.CompanyLocationModel = companyLocation.ToModel();
            }
            if (companyDepartment != null)
            {
                model.CompanyDepartmentModel = companyDepartment.ToModel();
            }
            if (schedulePolicy != null)
            {
                model.SchedulePolicyName = schedulePolicy.Name;
            }

            if (jobOrder.CompanyContactId != 0)
            {
                Account _companyContact = _companyContactService.GetCompanyContactById(jobOrder.CompanyContactId);
                model.CompanyContactModel = _companyContact.ToModel();
            }

            // Set PayRate, BillingRate, MaxRate to model from CompanyBilling model by BillingRateCode
            if (!String.IsNullOrWhiteSpace(model.BillingRateCode))
            {
                var _companyBilling = _companyBillingService.GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(model.CompanyId, model.CompanyLocationId, model.BillingRateCode, null);
                if (_companyBilling != null)
                {
                    model.BillingRate = _companyBilling.RegularBillingRate;
                    model.PayRate = _companyBilling.RegularPayRate;
                    model.OvertimeBillingRate = _companyBilling.OvertimeBillingRate;
                    model.OvertimePayRate = _companyBilling.OvertimePayRate;
                    model.MaxRate = _companyBilling.MaxRate;
                }
            }

            model.JobOrderStatusModel = jobOrder.JobOrderStatus.ToModel();
            model.JobOrderCategoryModel = jobOrder.JobOrderCategory.ToModel();
            model.JobOrderTypeModel = jobOrder.JobOrderType.ToModel();
            model.ShiftModel = jobOrder.Shift.ToModel();
            model.CompanyModel = jobOrder.Company.ToModel();
            model.ReferenceDate = refDate;

            Account recruiter = _accountService.GetAccountById(model.RecruiterId);
            if (recruiter != null)
                model.RecruiterName = recruiter.FullName;

            Account owner = _accountService.GetAccountById(model.OwnerId);
            if (owner != null)
                model.OwnerName = owner.FullName;

            return model;
        }

        public bool PermitViewDetaials(Wfm.Core.Domain.JobOrders.JobOrder jobOrder, IWorkContext _workContext, IRecruiterCompanyService _recruiterCompanyService, IPermissionService _permissionService)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return false;
            Account account = _workContext.CurrentAccount;
            if (account.IsAdministrator() || account.IsPayrollAdministrator() || account.IsOperator())
            {
                return true;
            }
            if (account.IsMSPRecruiter() || account.IsMSPRecruiterSupervisor())
            {
                List<int> ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                if (ids.Contains(jobOrder.CompanyId))
                    return true;                
                else                
                    return false;
            }
            if (account.IsVendorAdmin())
            {
                if (account.FranchiseId == jobOrder.FranchiseId)
                    return true;
                else
                    return false;
            }
            if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
            {
                if (account.FranchiseId != jobOrder.FranchiseId)
                    return false;
                List<int> companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                return companyIds.Contains(jobOrder.CompanyId);
            }
            return false;
        }

        public JobOrderModel CreateNewJobOrderModel(int companyId, IWorkContext _workContext,IJobOrderStatusService _jobOrderService)
        {
            JobOrderModel model = new JobOrderModel();
            if (companyId != 0)
                model.CompanyId = companyId;

            //model.JobOrderGuid = Guid.NewGuid();
            model.FranchiseId = _workContext.CurrentFranchise.Id;
            model.FranchiseGuid = _workContext.CurrentFranchise.FranchiseGuid;

            model.AllowSuperVisorModifyWorkTime = true;
            model.JobOrderStatusId = _jobOrderService.GetJobOrderStatusIdByName("Active").Value;
            model.RecruiterId = _workContext.CurrentAccount.Id;
            model.OwnerId = _workContext.CurrentAccount.Id;
            return model;
        }

        public bool PermitCreateJobOrder(IWorkContext _workContext, IPermissionService _permissionService)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders) && (_workContext.CurrentFranchise.EnableStandAloneJobOrders||_workContext.CurrentFranchise.IsDefaultManagedServiceProvider))
                return true;
            return false;
        }

        public bool PermitEditJobOrder(Wfm.Core.Domain.JobOrders.JobOrder jobOrder,IWorkContext _workContext, IPermissionService _permissionService)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return false;

            Account account = _workContext.CurrentAccount;
            if(account.IsAdministrator()||account.IsPayrollAdministrator())
                return true;

            if (jobOrder.JobOrderStatusId != (int)JobOrderStatusEnum.Closed)
            {
                if (account.IsVendorAdmin())
                {
                    if (jobOrder.FranchiseId == account.FranchiseId)
                        return true;
                    else
                        return false;
                }
                if (account.IsMSPRecruiter() || account.IsMSPRecruiterSupervisor() )
                {
                    if (jobOrder.RecruiterId == account.Id || jobOrder.OwnerId == account.Id)
                        return true;
                    else
                        return false;
                }
                if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
                {
                    if (jobOrder.FranchiseId != account.FranchiseId)
                        return false;
                    if (jobOrder.RecruiterId == account.Id || jobOrder.OwnerId == account.Id)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        
        public JobOrderModel GetACopyOfJobOrder(Wfm.Core.Domain.JobOrders.JobOrder jobOrder, IWorkContext _workContext)
        {
            JobOrderModel model = jobOrder.ToModel();
            
            model.Id = 0;
            model.JobTitle = "<<< COPY OF >>> " + model.JobTitle;
            model.JobOrderStatusId = (int)JobOrderStatusEnum.Active;
            model.StartDate = null; // to enforce validation
            //model.EndDate = null; // to enforce validation
            model.OwnerId = _workContext.CurrentAccount.Id;

            return model;
        }


        //public bool PermitCopyJobOrder(Wfm.Core.Domain.JobOrders.JobOrder jobOrder,IPermissionService _permissionService, IWorkContext _workContext)
        //{
        //    return PermitCreateJobOrder(_workContext, _permissionService);
        //}


        public string ValidateJobOrderModel(JobOrderModel model)
        {
            var result = String.Empty;

            // validate StartDate, EndDate
            if (!model.StartDate.HasValue)
                result = _localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Fail.InvalidStartEndDate");

            // validate StartTime, EndTime
            else if (!model.StartTime.HasValue || !model.EndTime.HasValue)
                result = _localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Fail.InvalidStartEndTime");

            else
            {
                // validate StartTime - EndTime
                DateTime jobStartDT = new DateTime(model.StartDate.Value.Year, model.StartDate.Value.Month, model.StartDate.Value.Day, model.StartTime.Value.Hour, model.StartTime.Value.Minute, 0);

                DateTime jobEndDate = model.StartDate.Value;
                if (model.StartTime.Value >= model.EndTime.Value)
                    jobEndDate = model.StartDate.Value.AddDays(1);
                DateTime jobEndDT = new DateTime(jobEndDate.Year, jobEndDate.Month, jobEndDate.Day, model.EndTime.Value.Hour, model.EndTime.Value.Minute, 0);

                // Calculate job order duration
                TimeSpan jobDurationTimeSpan = jobEndDT - jobStartDT;
                decimal jobDurationInHours = (decimal)jobDurationTimeSpan.TotalHours;
                if (jobDurationInHours < JOB_ORDER_MINIMUM_DURATION_IN_HOURS || jobDurationInHours > JOB_ORDER_MAXIMUM_DURATION_IN_HOURS)
                    result = _localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Fail.InvalidStartEndTime");
            }

            return result;
        }


        public Wfm.Core.Domain.JobOrders.JobOrder CreateJobOrderFromModel(JobOrderModel model)
        {
            Wfm.Core.Domain.JobOrders.JobOrder jobOrder = model.ToEntity();

            // set start/end time
            jobOrder.StartTime = jobOrder.StartDate.Date + jobOrder.StartTime.TimeOfDay;
            jobOrder.EndTime = jobOrder.StartDate.AddDays(jobOrder.EndTime.TimeOfDay < jobOrder.StartTime.TimeOfDay ? 1 : 0).Date + jobOrder.EndTime.TimeOfDay;

            //assign default value
            jobOrder.EnteredBy = _workContext.CurrentAccount.Id;
            jobOrder.IsDeleted = false;
            //jobOrder.Company = _companyService.GetCompanyById(jobOrder.CompanyId);
            _jobOrderService.InsertJobOrder(jobOrder);
            jobOrder = _jobOrderService.GetJobOrderById(jobOrder.Id);
            return jobOrder;
        }

        public int EditJobOrder(JobOrderModel model, int[] requiredTests, IPermissionService _permissionService, out string ErrorMessage)
        {
            int errorCode = 0; //0 means no errors found
            ErrorMessage = String.Empty;
            bool movePipelineDates = false;

            Wfm.Core.Domain.JobOrders.JobOrder jobOrder = _jobOrderService.GetJobOrderById(model.Id);
            if (jobOrder == null || jobOrder.FranchiseId != model.FranchiseId)
                return -1;

            if (!this.PermitEditJobOrder(jobOrder, _workContext, _permissionService))
                return -2;

            #region Key data validation

            ErrorMessage = this.ValidateJobOrderModel(model);
            if (!String.IsNullOrEmpty(ErrorMessage))
                return -3;

            // Is start Date changed? If start date is moved forward to a later date, then all assignments in the pipeline have to move too.
            // Also, if there are any submitted hours, the start date cannot change.
            if (model.StartDate > jobOrder.StartDate)
            {
                // If there are any submitted hours, the start date cannot change.
                if (_jobOrderService.JobOrderHasWorkTimeInDateRange(jobOrder.Id, jobOrder.StartDate, model.StartDate.Value.AddDays(-1)))
                {
                    ErrorMessage = _localizationService.GetResource("Admin.JobOrder.Fail.StartDateHasTimeSheet"); 
                    return -3;
                }

                if (_jobOrderService.PipelineHasDataInDateRange(jobOrder.Id, jobOrder.StartDate, model.StartDate.Value.AddDays(-1)) )
                {
                    ErrorMessage = _localizationService.GetResource("Admin.JobOrder.Fail.StartDateHasPlacement");  
                    return -3;
                }

                movePipelineDates = true;
            }

            #endregion

            #region Warning message

            ErrorMessage = this.CheckJobOrderCompleteness(model);

            #endregion

            jobOrder = this.UpdateJobOrderFromModel(jobOrder, model);

            //tests
            this.UpdateJobOrderTestCategories(jobOrder.Id, requiredTests);

            // Is start Date changed? If start date is moved forward to a later date, then all assignments in the pipeline have to move too.
            if (movePipelineDates)
            {
                _jobOrderService.UpdatePipeLineDateRange(jobOrder.Id, model.StartDate.Value); // Update the pipeleine
            }

            return errorCode;
        }

        public Wfm.Core.Domain.JobOrders.JobOrder UpdateJobOrderFromModel(Wfm.Core.Domain.JobOrders.JobOrder jobOrder, JobOrderModel model)
        {
            jobOrder = model.ToEntity(jobOrder);

            jobOrder.UpdatedOnUtc = System.DateTime.UtcNow;

            _jobOrderService.UpdateJobOrder(jobOrder);

            return jobOrder;
        }

        public void AddJobOrderTestCategories(int jobOrderId, int[] RequiredTests)
        {
            foreach (var testCategoryId in RequiredTests)
            {
                JobOrderTestCategory _JobOrderTestCategory = new JobOrderTestCategory()
                {
                    JobOrderId = jobOrderId,
                    TestCategoryId = testCategoryId,
                    IsActive = true,
                    EnteredBy = _workContext.CurrentAccount.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                };

                _jobOrderTestCategoryService.InsertJobOrderTestCategory(_JobOrderTestCategory);
            }
        }


        public void UpdateJobOrderTestCategories(int jobOrderId, int[] RequiredTests)
        {
            //tests
            IList<JobOrderTestCategory> existingTestList = _jobOrderTestCategoryService.GetJobOrderTestCategoryByJobOrderId(jobOrderId);

            if (RequiredTests == null)
            {
                //clean up all
                foreach (JobOrderTestCategory item in existingTestList)
                    _jobOrderTestCategoryService.DeleteJobOrderTestCategory(item);
            }
            else
            {
                //clean up
                List<int> requiredTestIdList = RequiredTests.ToList();
                foreach (JobOrderTestCategory item in existingTestList)
                {
                    if (!requiredTestIdList.Contains(item.TestCategoryId))
                        _jobOrderTestCategoryService.DeleteJobOrderTestCategory(item);
                }

                List<int> existingTestIdList = existingTestList.Select(d => d.TestCategoryId).ToList();
                foreach (var testCategoryId in RequiredTests)
                {
                    //already exists
                    if (existingTestIdList.Contains(testCategoryId))
                        continue;

                    //new item
                    JobOrderTestCategory jobOrderTestCategory = new JobOrderTestCategory()
                    {
                        JobOrderId = jobOrderId,
                        TestCategoryId = testCategoryId,
                        IsActive = true,
                        EnteredBy = _workContext.CurrentAccount.Id,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    };

                    _jobOrderTestCategoryService.InsertJobOrderTestCategory(jobOrderTestCategory);
                }
            }
        }


        public string CheckJobOrderCompleteness(JobOrderModel jobOrder)
        {
            var result = new List<string>();

            if (jobOrder.CompanyLocationId == 0)
                result.Add(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Warning.NoCompanyLocation"));
            
            if (!jobOrder.CompanyDepartmentId.HasValue || jobOrder.CompanyDepartmentId.Value == 0)
                result.Add(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Warning.NoCompanyDepartment"));

            if (!jobOrder.CompanyContactId.HasValue || jobOrder.CompanyContactId.Value == 0)
                result.Add(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Warning.NoCompanyContact"));
            else
            {
                var contact = _accountService.GetAccountById(jobOrder.CompanyContactId.Value);
                if (contact != null && (contact.CompanyLocationId != jobOrder.CompanyLocationId || contact.CompanyDepartmentId != jobOrder.CompanyDepartmentId))
                    result.Add(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Warning.OrgSettingsNotMatch"));
            }

            if (jobOrder.SchedulePolicyId == 0)
                result.Add(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Warning.NoSchedulePolicy"));

            if (String.IsNullOrWhiteSpace(jobOrder.BillingRateCode))
                result.Add(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added.Warning.NoBillingRate"));

            return String.Join(" | ", result);
        }
    }
}
