using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Services.JobOrders;
using Wfm.Web.Framework;
using Wfm.Admin.Extensions;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Accounts;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Seo;
using Wfm.Shared.Models.Common;
using Wfm.Services.Candidates;
using Wfm.Core.Domain.Candidates;
using Wfm.Admin.Models.Candidate;
using Wfm.Services.Franchises;
using Wfm.Services.Companies;


namespace Wfm.Admin.Models.JobOrder
{
    public class DirectHireJobOrder_BL
    {
        #region fields
        private readonly IDirectHireJobOrderService _directHireJobOrderService;
        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;       
        private readonly ILocalizationService _localizationService;
        private readonly IAccountService _accountService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateDirectHireStatusHistoryService _candidateDirectHireStatusHistoryService;
        private readonly IJobOrderStatusService _jobOrderStatusService;
        private readonly IFranchiseService _franchiseService;
        private readonly ICompanyService _companyService;
        #endregion

        #region Ctor
        public DirectHireJobOrder_BL(IWorkContext workContext, 
                                     IDirectHireJobOrderService directHireJobOrderService, 
                                     IActivityLogService activityLogService, 
                                     ILocalizationService localizationService, 
                                     IUrlRecordService urlRecordService, 
                                     IAccountService accountService, 
                                     ICandidateDirectHireStatusHistoryService candidateDirectHireStatusHistoryService, 
                                     ICandidateJobOrderService candidateJobOrderService, 
                                     IJobOrderStatusService jobOrderStatusService, 
                                     IFranchiseService franchiseService, 
                                     ICompanyService companyService)
        {
            _workContext = workContext;
            _directHireJobOrderService = directHireJobOrderService;
            _activityLogService = activityLogService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _accountService = accountService;
            _candidateDirectHireStatusHistoryService = candidateDirectHireStatusHistoryService;
            _candidateJobOrderService = candidateJobOrderService;
            _jobOrderStatusService = jobOrderStatusService;
            _franchiseService = franchiseService;
            _companyService = companyService;
        }
        #endregion


        public DataSourceResult GetAllDirectHireJobOrderList([DataSourceRequest] DataSourceRequest request, DateTime startDate, DateTime endDate) 
        {
            var directHireJobOrders = _directHireJobOrderService.GetDirectHireJobOrderList(_workContext.CurrentAccount, startDate, endDate).PagedForCommand(request);

            var directHireJobOrdersList = new List<DirectHireJobOrderListModel>();
            foreach (var x in directHireJobOrders)
            {
                var directHireJobOrderModel = x.ToModel();
                directHireJobOrdersList.Add(directHireJobOrderModel); 
            }

            var result = new DataSourceResult()
            {
                Data = directHireJobOrdersList, // Process data (paging and sorting applied)
                Total = directHireJobOrders.TotalCount, // Total number of records
            };
            return result;
        }

        public DataSourceResult GetDirectHireCandidatePoolList([DataSourceRequest] DataSourceRequest request)
        {
            var skills = _directHireJobOrderService.GetDirectHireCandidatePoolList().PagedForCommand(request);

            var result = new DataSourceResult()
            {
                Data = skills.Select(x => x.ToModel()),
                Total = skills.TotalCount
            };
            return result; 
        }

        public DirectHireJobOrderModel CreateNewDirectHireJobOrderModel()
        {
            DirectHireJobOrderModel model = new DirectHireJobOrderModel();          
            model.FranchiseId = _workContext.CurrentFranchise.Id;
            model.FranchiseGuid = _workContext.CurrentFranchise.FranchiseGuid;     
            model.RecruiterId = _workContext.CurrentAccount.Id;
            model.OwnerId = _workContext.CurrentAccount.Id;
            model.JobOrderStatusId = _jobOrderStatusService.GetJobOrderStatusIdByName("Active").Value;
            return model;
        }

        public Wfm.Core.Domain.JobOrders.JobOrder CreateDirectHireJobOrder(DirectHireJobOrderModel model)
        {
            model.EnteredBy = _workContext.CurrentAccount.Id;

            var company = _companyService.GetCompanyByGuid(model.CompanyGuid);
            if (company != null)
                model.CompanyId = company.Id;
            var franchise = _franchiseService.GetFranchiseByGuid(model.FranchiseGuid);
            if (franchise != null)
                model.FranchiseId = franchise.Id;
            Wfm.Core.Domain.JobOrders.JobOrder directHireJobOrder = model.ToEntity();

           //default values for JobOrder
            directHireJobOrder.SchedulePolicyId = 0;
            directHireJobOrder.ShiftId = 1;
            directHireJobOrder.StartTime = model.StartDate.Value.Date.AddHours(8);
            directHireJobOrder.EndTime = model.StartDate.Value.Date.AddHours(16);
            directHireJobOrder.CompanyContactId = 0;
            directHireJobOrder.BillingRateCode = "DirectPlacement";

            _directHireJobOrderService.InsertDirectHireJobOrder(directHireJobOrder);
            directHireJobOrder = _directHireJobOrderService.GetDirectHireJobOrderById(directHireJobOrder.Id);
            DirectHireJobOrderModel jobOrderSEO = directHireJobOrder.ToDirectHireJobOrderModel();
            jobOrderSEO.SeName = directHireJobOrder.Id + "-" + directHireJobOrder.JobTitle.ToLower().Replace(" ", "-");
            _urlRecordService.SaveSlug(directHireJobOrder, jobOrderSEO.SeName, 1);
           
            //activity log
            _activityLogService.InsertActivityLog("AddNewDirectHireJobOrder", _localizationService.GetResource("ActivityLog.AddNewDirectHireJobOrder"), directHireJobOrder.Id);

            return directHireJobOrder; 
        }
        public DirectHireJobOrderModel GetEditDirectHireJobOrder(Guid guid)
        {
            Wfm.Core.Domain.JobOrders.JobOrder directHireJobOrder = _directHireJobOrderService.GetDirectHireJobOrderByGuid(guid);
            if (directHireJobOrder == null)
                return null;

            DirectHireJobOrderModel model = directHireJobOrder.ToDirectHireJobOrderModel();
            
            var franchise = _franchiseService.GetFranchiseById(model.FranchiseId??0);
            if (franchise != null)
                model.FranchiseGuid = franchise.FranchiseGuid;
            return model;
        }

        public DirectHireJobOrderModel GetDirectHireJobOrderDetail(Guid? guid)
        {
            Wfm.Core.Domain.JobOrders.JobOrder directHireJobOrder = _directHireJobOrderService.GetDirectHireJobOrderByGuid(guid);
            if (directHireJobOrder == null)
                return null;

            DirectHireJobOrderModel model = directHireJobOrder.ToDirectHireJobOrderModel();
            
            if (model.JobOrderCategoryId > 0)
                model.JobOrderCategoryModel = directHireJobOrder.JobOrderCategory.ToModel();
            if (model.JobOrderTypeId > 0)
                model.JobOrderTypeModel = directHireJobOrder.JobOrderType.ToModel();
            Account recruiter = _accountService.GetAccountById(Convert.ToInt32(model.RecruiterId));
            if (recruiter != null)
                model.RecruiterName = recruiter.FullName;

            Account owner = _accountService.GetAccountById(Convert.ToInt32(model.OwnerId));
            if (owner != null)
                model.OwnerName = owner.FullName;
             
            return model;
        }
        public Wfm.Core.Domain.JobOrders.JobOrder UpdateDirectHireJobOrder(DirectHireJobOrderModel model, Wfm.Core.Domain.JobOrders.JobOrder directHireJobOrder)
        {
            model.EnteredBy = directHireJobOrder.EnteredBy;
            model.IsDeleted = directHireJobOrder.IsDeleted;
            model.JobOrderGuid = directHireJobOrder.JobOrderGuid;
            var company = _companyService.GetCompanyByGuid(model.CompanyGuid);
            if (company != null)
                model.CompanyId = company.Id;
            var franchise = _franchiseService.GetFranchiseByGuid(model.FranchiseGuid);
            if (franchise != null)
                model.FranchiseId = franchise.Id;
            directHireJobOrder = model.ToEntity(directHireJobOrder);

            directHireJobOrder.SchedulePolicyId = 0;
            directHireJobOrder.ShiftId = 1;

            _directHireJobOrderService.UpdateDirectHireJobOrder(directHireJobOrder);
            //activity log
            _activityLogService.InsertActivityLog("UpdateDirectHireJobOrder", _localizationService.GetResource("ActivityLog.UpdateDirectHireJobOrder"), directHireJobOrder.Id);
            return directHireJobOrder;
        }
        public JobOrderCandidateModel GetJobOrderCandidateModel(Guid? guid)
        {

            var jobOrder = _directHireJobOrderService.GetDirectHireJobOrderByGuid(guid);
            if (jobOrder == null)
                return null;

            var model = new JobOrderCandidateModel()
            {
                JobOrderId = jobOrder.Id,
                JobOrderGuid = jobOrder.JobOrderGuid,
                CompanyGuid = jobOrder.Company.CompanyGuid,
                CompanyId = jobOrder.CompanyId,
                RecruiterId = jobOrder.RecruiterId,
                OwnerId = jobOrder.OwnerId
            };
            return model;
        }

        public IList<CandidatePipelineSimpleModel> DirectHireJobOrderPlacedCandidates(Guid? guid)
        {
            var jobOrder = _directHireJobOrderService.GetDirectHireJobOrderByGuid(guid); 
            if (jobOrder == null)
            {               
                return null;
            }
            var candidateJobOrder = _candidateJobOrderService.GetCandidateJobOrderByJobOrderId(jobOrder.Id, null);
            var candidatePipelineSimpleList = new List<CandidatePipelineSimpleModel>();
            if (candidateJobOrder != null)
            {
                foreach (var item in candidateJobOrder)
                {
                    CandidatePipelineSimpleModel i = new CandidatePipelineSimpleModel();
                    i.CandidateGuid = item.Candidate.CandidateGuid;
                    i.CandidateId = item.CandidateId;
                    i.JobOrderId = item.JobOrderId;
                    i.Id = item.Id;
                    i.FirstName = item.Candidate.FirstName;
                    i.LastName = item.Candidate.LastName;
                    i.HomePhone = item.Candidate.HomePhone;
                    i.MobilePhone = item.Candidate.MobilePhone;
                    i.Email = item.Candidate.Email;
                    i.RatingValue = item.RatingValue;
                    i.RatingComment = item.RatingComment;
                    i.StatusName = item.CandidateJobOrderStatus.StatusName;
                    i.CreatedOnUtc = item.CreatedOnUtc;
                    i.UpdatedOnUtc = item.UpdatedOnUtc;
                    i.CandidateJobOrderId = item.Id;
                    i.EmployeeId = item.Candidate.EmployeeId;
                    candidatePipelineSimpleList.Add(i);
                }
            }

            return candidatePipelineSimpleList;
        }

        public string AddCandidateIntoPipeline(int jobOrderId, int candidateId)
        {          
            if (_candidateJobOrderService.IsCandidateInJobOrderPipeline(jobOrderId,candidateId))
            {
                return "Candidate already placed in job order";
            }
            var newCjo = new CandidateJobOrder()
            {
                CandidateId = candidateId,
                JobOrderId = jobOrderId,
                StartDate = DateTime.Now.Date,
                EndDate = null,
                UpdatedOnUtc = DateTime.UtcNow,
                CreatedOnUtc = DateTime.UtcNow,
                EnteredBy = _workContext.CurrentAccount.Id,
                CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.DirectHireSubmitted
            };
            _candidateJobOrderService.InsertCandidateJobOrder(newCjo);

            var newCjoStatus = new CandidateDirectHireStatusHistory()
            {
                CandidateId = candidateId,
                JobOrderId = jobOrderId,
                StatusFrom = 0,
                StatusTo = (int)CandidateJobOrderStatusEnum.DirectHireSubmitted,
                UpdatedOnUtc = DateTime.UtcNow,
                CreatedOnUtc = DateTime.UtcNow,
                EnteredBy = _workContext.CurrentAccount.Id
            };
            _candidateDirectHireStatusHistoryService.InsertCandidateDirectHireStatusHistory(newCjoStatus);
            return string.Empty;
        }

        public CandidateDirectHireStatusHistoryModel InsertCandidateDirectHireStatusHistory(CandidateDirectHireStatusHistoryModel model, int candidateId, int jobOrderId)
        {
            CandidateJobOrder currentRecord = _candidateJobOrderService.GetCandidateJobOrderByJobOrderIdAndCandidateId(jobOrderId, candidateId);

            model.CandidateId = candidateId;
            model.JobOrderId = jobOrderId;
            model.UpdatedOnUtc = DateTime.UtcNow;
            model.CreatedOnUtc = DateTime.UtcNow;
            model.EnteredBy = _workContext.CurrentAccount.Id;
            model.EnteredName = _workContext.CurrentAccount.FullName;
            model.StatusFrom = currentRecord.CandidateJobOrderStatusId;
            var entity = model.ToEntity();
            _candidateDirectHireStatusHistoryService.InsertCandidateDirectHireStatusHistory(entity);
            if (currentRecord.CandidateJobOrderStatusId != model.StatusTo)
            {
                currentRecord.CandidateJobOrderStatusId = model.StatusTo;
                currentRecord.UpdatedOnUtc = DateTime.UtcNow;
                _candidateJobOrderService.UpdateCandidateJobOrder(currentRecord);
            }
            model.Id = entity.Id;
            return model;
        }
        public void UpdateCandidateDirectHireStatusHistory(CandidateDirectHireStatusHistoryModel model, int candidateId, int jobOrderId)
        {
            var entity = _candidateDirectHireStatusHistoryService.GetCandidateDirectHireStatusHistoryById(model.Id);
            int oldStatus = entity.StatusTo;
            model.ToEntity(entity);
            _candidateDirectHireStatusHistoryService.UpdateCandidateDirectHireStatusHistory(entity);
            if (oldStatus != model.StatusTo)
            {
                CandidateJobOrder currentRecord = _candidateJobOrderService.GetCandidateJobOrderByJobOrderIdAndCandidateId(jobOrderId, candidateId);
                currentRecord.CandidateJobOrderStatusId = model.StatusTo;
                currentRecord.UpdatedOnUtc = DateTime.UtcNow;
                _candidateJobOrderService.UpdateCandidateJobOrder(currentRecord);
            }
        }
        public DataSourceResult GetCandidateDirectHireStatusHistory([DataSourceRequest] DataSourceRequest request, int candidateId, int jobOrderId)
        {
            var candidateDirectHireStatusHistoryList = _candidateDirectHireStatusHistoryService.GetCandidateDirectHireStatusHistoryByCandidateAndJobOrderId(candidateId, jobOrderId)
                                                       .ToList().Select(x => x.ToModel()).ToList();

            foreach (var h in candidateDirectHireStatusHistoryList)
            {
                var enteredByAccount = _accountService.GetAccountById(h.EnteredBy);
                h.EnteredName = enteredByAccount == null ? string.Empty : enteredByAccount.FullName;
            }

            return candidateDirectHireStatusHistoryList.ToDataSourceResult(request);
        }


        public IList<CandidateDirectHireStatusHistory> GetCandidateDirectHireStatusHistoriesByPipelineIds(string selectedIds)
        {
            var result = new List<CandidateDirectHireStatusHistory>();

            if (!String.IsNullOrWhiteSpace(selectedIds))
            {
                var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
                var hired = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable()
                            .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Hired && ids.Contains(x.Id))
                            .Select(x => new 
                            {
                                CandidateId = x.CandidateId,
                                JobOrderId = x.JobOrderId
                            });
                var placements = _candidateDirectHireStatusHistoryService.GetAllCandidateDirectHireStatusHistoriesAsQueryable()
                                 .Where(x => x.StatusTo == (int)CandidateJobOrderStatusEnum.Hired && x.IssueInvoice);

                result = (from p in placements
                          join h in hired on new { x = p.CandidateId, y = p.JobOrderId } equals new { x = h.CandidateId, y = h.JobOrderId }
                          select p).ToList();
            }

            return result;
        }
    }
}