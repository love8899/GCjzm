using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Candidate;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Messages;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Models.Common;
using Wfm.Web.Framework;


namespace Wfm.Admin.Models.JobOrder
{
    public class JobOrderCandidateModel_BL
    {
        #region Fields
        
        private readonly IJobOrderService _jobOrderService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IWorkTimeService _workTimeService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly ICandidateService _candidatesService;
        private readonly ICandidateAvailabilityService _availabilityService;
        private readonly IAccountService _accountService;
        private readonly IFranchiseService _franchiseService;
        private readonly IWorkContext _workContext;
        private readonly ICompanyService _companyService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion


        #region ctor

        public JobOrderCandidateModel_BL(IJobOrderService jobOrderService,
                                        ICandidateJobOrderService candidateJobOrderService,
                                        IWorkTimeService worktimeService,
                                        ICompanyCandidateService companyCandidateService,
                                        ICandidateService candidatesService,
                                        ICandidateAvailabilityService availabilityService,
                                        IAccountService accountService,
                                        IFranchiseService franchiseService,
                                        IWorkContext workContext,
                                        ICompanyService companyService,
                                        IWorkflowMessageService workflowMessageService)
        {
            _candidateJobOrderService = candidateJobOrderService;
            _jobOrderService = jobOrderService;
            _workTimeService = worktimeService;
            _companyCandidateService = companyCandidateService;
            _candidatesService = candidatesService;
            _availabilityService = availabilityService;
            _accountService = accountService;
            _franchiseService = franchiseService;
            _workContext = workContext;
            _companyService = companyService;
            _workflowMessageService = workflowMessageService;
        }
        
        #endregion


        #region Method
        
        public JobOrderCandidateModel GetJobOrderCandidateModel(Guid? guid, out bool isShort, string inquiryDateString = null, bool includePlacedCandidates = false)
        {
            isShort = false;
            var inquiryDate = DateTime.Today;
            if (!string.IsNullOrEmpty(inquiryDateString)) 
                DateTime.TryParse(inquiryDateString, out inquiryDate);

            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
                return null;
            isShort = jobOrder.EndDate != null && (jobOrder.EndDate.Value - jobOrder.StartDate).Days <= 6 && jobOrder.EndDate.Value.DayOfWeek >= jobOrder.StartDate.DayOfWeek;

            int placedCount = _candidateJobOrderService.GetNumberOfPlacedCandidatesByJobOrder(jobOrder.Id, inquiryDate);
            JobOrderOpening[] _openingChanges;
            int openingAvailable = _jobOrderService.GetJobOrderOpeningAvailable(jobOrder.Id, inquiryDate, out _openingChanges);
            //var accounts = _accountService.GetAllAccountsAsQueryable();
            //var vendors = _franchiseService.GetAllFranchisesAsQueryable();
            //JobOrderOpeningModel[] openingChanges = (from o in _openingChanges
            //                                         join a in accounts on o.EnteredBy equals a.Id
            //                                         join v in vendors on a.FranchiseId equals v.Id
            //                                         orderby o.StartDate descending
            //                                         select new JobOrderOpeningModel() {
            //                                                 OpeningAvailable = o.OpeningNumber,
            //                                                 StartDate = o.StartDate,
            //                                                 ChangedDateTime = o.UpdatedOnUtc.GetValueOrDefault().ToLocalTime(),
            //                                                 ChangedBy = a.FirstName + " " + a.LastName + " of " + v.FranchiseName,
            //                                                 Note = o.Note
            //                                        }).ToArray();

            var model = new JobOrderCandidateModel()
            {
                JobOrderId = jobOrder.Id,
                JobOrderGuid = jobOrder.JobOrderGuid,
                CompanyGuid = jobOrder.Company.CompanyGuid,
                CompanyId = jobOrder.CompanyId,
                InquiryDate = inquiryDate,
                OpeningAvaliable = openingAvailable,
                Placed = placedCount, 
                //ListOfOpenningChanges = openingChanges,
                RecruiterId = jobOrder.RecruiterId,
                OwnerId = jobOrder.OwnerId
            };

            return model;
        }


        public IList<CandidatePipelineSimpleModel> _TabJobOrderPipelinePlaced(Guid? guid, DateTime inquiryDate, out int openingAvailable, out JobOrderOpeningModel[] openingChanges)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if(jobOrder==null)
            {
                openingAvailable = 0;
                openingChanges = new JobOrderOpeningModel[1];
                return null;
            }

            var candidateJobOrder = _candidateJobOrderService.GetCandidateJobOrderByJobOrderId(jobOrder.Id, inquiryDate);
            var placedCandidateJobOrderModel = new List<CandidatePipelineSimpleModel>();
            var otherCandidateJobOrderModel = new List<CandidatePipelineSimpleModel>();
            
            JobOrderOpening[] _openingChanges;
            openingAvailable = _jobOrderService.GetJobOrderOpeningAvailable(jobOrder.Id, inquiryDate, out _openingChanges);
            openingChanges = _openingChanges.Select(x => new JobOrderOpeningModel() { OpeningAvailable = x.OpeningNumber, StartDate = x.StartDate, ChangedDateTime = x.UpdatedOnUtc.GetValueOrDefault().ToLocalTime(), Note = x.Note }).ToArray();

            int _PlacedStatusIdAsInt = (int)CandidateJobOrderStatusEnum.Placed;

            if (candidateJobOrder != null)
            {
                foreach (var item in candidateJobOrder)
                {
                    if ((otherCandidateJobOrderModel.Exists(x => x.CandidateId == item.CandidateId) && item.CandidateJobOrderStatusId != _PlacedStatusIdAsInt) ||
                        (placedCandidateJobOrderModel.Exists(x => x.CandidateId == item.CandidateId) ) ||
                         (!item.Candidate.IsActive && item.CandidateJobOrderStatusId != _PlacedStatusIdAsInt)
                       )
                    {
                        // This is to avoid duplicate records for the same candidate in the grid
                        continue;
                    }

                    CandidatePipelineSimpleModel i = new CandidatePipelineSimpleModel();
                    i.CandidateGuid = item.Candidate.CandidateGuid;
                    i.CandidateId = item.CandidateId;
                    i.EmployeeId = item.Candidate.EmployeeId;
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
                    
                    PopulateWorktTime(i, inquiryDate);
                    PopulateAvailability(i, inquiryDate);
                    if (item.CandidateJobOrderStatusId == _PlacedStatusIdAsInt)
                    {
                        placedCandidateJobOrderModel.Add(i);
                    }
                    else
                    {
                        otherCandidateJobOrderModel.Add(i);
                    }
                }
            }

            return placedCandidateJobOrderModel.Union(otherCandidateJobOrderModel).ToList();
        }


        private void PopulateWorktTime(CandidatePipelineSimpleModel model, DateTime inquiryDate)
        {
            var entry = _workTimeService.GetAllWorkTimeByJobOrderAndDateAsQueryable(model.JobOrderId, inquiryDate)
                .FirstOrDefault(x => x.CandidateId == model.CandidateId);
            if (entry != null)
            {
                model.JobDuration = entry.JobOrderDurationInHours;
                model.IsArrivedToday = model.JobDuration > 0m;
            }

            var oneYearAgo = inquiryDate.AddYears(-1);
            var workTimes = _workTimeService.GetWorkTimeByStartEndDateAsQueryable(oneYearAgo, inquiryDate)
                .Where(x => x.CandidateId == model.CandidateId)
                .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided)
                .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Rejected);
            model.TotalHours = workTimes.Sum(x => (decimal?)x.NetWorkTimeInHours) ?? 0m;
        }


        private void PopulateAvailability(CandidatePipelineSimpleModel model, DateTime inquiryDate)
        {
            var availability = _availabilityService.GetAllCandidateAvailability(inquiryDate, inquiryDate)
                .FirstOrDefault(x => x.CandidateId == model.CandidateId);
            if (availability != null)
                model.AvailableShiftId = availability.ShiftId;
        }

        #endregion


        #region GetGlobalEmployees

        public DataSourceResult GetCandidatesFromGlobalPool(DataSourceRequest request,DateTime inquiryDate, int companyId)
        {
            string filterJobOrderId = request.ExtractFilterValue("CandidateJobOrderModel.JobOrderId");
            string filterCityId = request.ExtractFilterValue("CandidateAddressModel.CityId");
            var candidates = _candidatesService.GetAllCandidatesAsQueryableByJobOrderIdAndCityIdFilters(_workContext.CurrentAccount, inquiryDate, companyId, filterJobOrderId, filterCityId)
                             .PagedForCommand(request);
            var candidateModelList = new List<CandidatePoolModel>();

            foreach (var item in candidates)
            {
                var candidateModel = item.ToPoolModel();
                var activeJobOrders = _candidateJobOrderService.GetCandidateJobOrderByCandidateIdAsQueryable(item.Id)
                    .Where(x => !x.EndDate.HasValue || x.EndDate > DateTime.Today);
                if(activeJobOrders.Count()>0)
                    candidateModel.CandidateJobOrderModel = activeJobOrders.FirstOrDefault().ToModel();
                //candidateModel.SocialInsuranceNumber = item.SocialInsuranceNumber.ToMaskedSocialInsuranceNumber();
                var address = item.CandidateAddresses.Where(o => o.AddressTypeId == (int)AddressTypeEnum.Residential).FirstOrDefault();
                if (address == null)
                {
                    candidateModel.CandidateAddressModel = new CandidateAddressModel() { CandidateId = item.Id };
                }
                else
                {
                    candidateModel.CandidateAddressModel = address.ToModel();
                }
                candidateModelList.Add(candidateModel);
            }
            var result = new DataSourceResult()
            {
                Data = candidateModelList, // Process data (paging and sorting applied)
                Total = candidates.TotalCount, // Total number of records
            };
            return result;
        }

        #endregion


        public string ToggleCandidatePipelineStatus(int candidateJobOrderId, DateTime placementDate, out string buttonCaption)
        {
            string errorMessage = String.Empty;

            CandidateJobOrder currentRecord = _candidateJobOrderService.GetCandidateJobOrderById(candidateJobOrderId);

            int oldStatusId = currentRecord.CandidateJobOrderStatusId;
            // true: the status is changing into 'Placed'.     false: the status is changing into 'No Status'
            bool actionIsPlacing = !(oldStatusId == (int)CandidateJobOrderStatusEnum.Placed);
            int newStatusId = actionIsPlacing ? (int)CandidateJobOrderStatusEnum.Placed : (int)CandidateJobOrderStatusEnum.NoStatus;
            buttonCaption = actionIsPlacing ? "Activate" : "Deactivate"; //text is not changing

            if (actionIsPlacing)
            {
                currentRecord.StartDate = placementDate;
                currentRecord.CandidateJobOrderStatusId = newStatusId;
               // currentRecord.EndDate = null;
                errorMessage =  _candidateJobOrderService.CreateNewPlacement(currentRecord, 0, false);

                //errorMessage = _candidateJobOrderService.SetCandidateJobOrderToPlaced(
                //                    currentRecord.CandidateId, currentRecord.JobOrderId, currentRecord.StartDate, currentRecord.EndDate, currentRecord.CandidateJobOrderStatusId, 
                //                                           placementDate, currentRecord.EndDate);
            }
            else
                errorMessage = _candidateJobOrderService.SetCandidateJobOrderToNoStatus(currentRecord.CandidateId, currentRecord.JobOrderId, currentRecord.CandidateJobOrderStatusId, placementDate, currentRecord.EndDate);

            if (String.IsNullOrWhiteSpace(errorMessage))
            {
                buttonCaption = actionIsPlacing ? "Deactivate" : "Activate"; //text after the change;
            }

            return errorMessage;
        }


        public string AddCandidateIntoPipeline(int jobOrderId, int candidateId, DateTime startDate, DateTime? endDate = null, bool addToCompanyPool = true)
        {
            var newCjo = new CandidateJobOrder()
            {
                CandidateId = candidateId,
                JobOrderId = jobOrderId,
                StartDate = startDate,
                EndDate = endDate,
                CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.NoStatus,
                EnteredBy = _workContext.CurrentAccount.Id
            };

            return _candidateJobOrderService.CreateNewPlacement(newCjo);
        }


        public string RemoveCandidateFromPipeline(int jobOrderId, int candidateId, DateTime refDate)
        {
            var cjo = _candidateJobOrderService.GetAllCandidateJobOrdersByJobOrderIdAndCandidateId(jobOrderId, candidateId)
                      .Where(x => x.StartDate <= refDate && (!x.EndDate.HasValue || x.EndDate >= refDate))
                      .FirstOrDefault();

            if (refDate == cjo.StartDate)
                return _candidateJobOrderService.RemovePlacement(cjo);
            
            var newCjo = new CandidateJobOrder()
            {
                Id = cjo.Id,
                CandidateId = candidateId,
                JobOrderId = jobOrderId,
                StartDate = cjo.StartDate,
                EndDate = refDate.AddDays(-1),
                CandidateJobOrderStatusId = cjo.CandidateJobOrderStatusId,
                EnteredBy = _workContext.CurrentAccount.Id
            };

            return _candidateJobOrderService.UpdateExistingPlacement(newCjo, keepLeftOver: false);
        }


        public string AddCandidatesFromGlobalPoolIntoJobOrder(int jobOrderId, int[] candidateIds, DateTime startDate, bool terminateCurrentPlacement = true, bool addToCompanyPool = true)
        {
            if (candidateIds == null)
                return "Please select at least one candidate!";

            var errorMessage = String.Empty;

            try
            {
                var notToDoList = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable()
                                    .Where(x => x.JobOrderId == jobOrderId && candidateIds.Contains(x.CandidateId) && 
                                                x.StartDate <= startDate && (!x.EndDate.HasValue || x.EndDate > startDate) && 
                                                x.CandidateJobOrderStatusId == (int)(CandidateJobOrderStatusEnum.Placed))
                                    .Select(x => x.CandidateId).ToArray();

                foreach (var candidateId in candidateIds.Except(notToDoList))
                {
                    AddCandidateIntoPipeline(jobOrderId, candidateId, startDate, addToCompanyPool: addToCompanyPool);

                    if (terminateCurrentPlacement)
                    {
                        var currentJobOrderIds = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable()
                                                    .Where(x => x.JobOrderId != jobOrderId && candidateId == x.CandidateId && 
                                                                x.StartDate <= startDate && (!x.EndDate.HasValue || x.EndDate > startDate) && 
                                                                x.CandidateJobOrderStatusId == (int)(CandidateJobOrderStatusEnum.Placed))
                                                    .Select(x => x.JobOrderId).ToArray();

                        foreach (var cId in currentJobOrderIds)
                            RemoveCandidateFromPipeline(cId, candidateId, startDate);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }


        public string SendDailyConfirmation(Guid jobOrderGuid, DateTime refDate, out int done, List<int> statusList)
        {
            done = 0;
            var jobOrder = _jobOrderService.GetJobOrderByGuid(jobOrderGuid);
            if (jobOrder == null)
                return "The job order can not be found.";

            return _workflowMessageService.SendDailyConfirmationToCandidate(jobOrder, refDate, out done, statusList);
        }
    }
}
