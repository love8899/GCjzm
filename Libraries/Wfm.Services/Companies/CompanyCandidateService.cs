using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Data;
using Wfm.Services.Candidates;
using Wfm.Services.Configuration;


namespace Wfm.Services.Companies
{
    public partial class CompanyCandidateService : ICompanyCandidateService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<CompanyCandidate> _companyCandidateRepository;
        private readonly IRepository<CompanyLocation> _companyLocationRepository;
        private readonly IRepository<Shift> _shifts;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly IRepository<EmployeeType> _employeeTypes;
        private readonly IRepository<JobOrder> _jobOrders;
        private readonly IRepository<CandidateJobOrder> _placements;
        private readonly IRepository<CandidateWorkTime> _candidateWorkTimeRepository;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;
        private readonly ICandidateAvailabilityService _availabilityService;

        #endregion

        #region Ctor

        public CompanyCandidateService(
            IWorkContext workContext,
            IRepository<CompanyCandidate> companyCandidateRepository,
            IRepository<CompanyLocation> companyLocationRepository,
            IRepository<Shift> shifts,
            IRepository<Candidate> candidateRepository,
            IRepository<EmployeeType> employeeTypes,
            IRepository<JobOrder> jobOrders,
            IRepository<CandidateJobOrder> placements,
            IRepository<CandidateWorkTime> candidateWorkTimeRepository,
            IDbContext dbContext,
            CommonSettings commonSettings,
            ISettingService settingService,
            ICandidateAvailabilityService availabilityService)
        {
            _workContext = workContext;
            _companyCandidateRepository = companyCandidateRepository;
            _companyLocationRepository = companyLocationRepository;
            _shifts = shifts;
            _candidateRepository = candidateRepository;
            _employeeTypes = employeeTypes;
            _jobOrders = jobOrders;
            _placements = placements;
            _candidateWorkTimeRepository = candidateWorkTimeRepository;
            _dbContext = dbContext;
            _commonSettings = commonSettings;
            _settingService = settingService;
            _availabilityService = availabilityService;
        }

        #endregion

        #region CRUD

        public void InsertCompanyCandidate(CompanyCandidate companyCandidate)
        {
            if (companyCandidate == null) throw new ArgumentNullException("companyCandidate");
            _companyCandidateRepository.Insert(companyCandidate);
        }

        public void UpdateCompanyCandidate(CompanyCandidate companyCandidate)
        {
            if (companyCandidate == null) throw new ArgumentException("companyCandidate");
            _companyCandidateRepository.Update(companyCandidate);
        }

        public void DeleteCompanyCandidate(CompanyCandidate companyCandidate)
        {
            if (companyCandidate == null) throw new ArgumentException("companyCandidate");
            _companyCandidateRepository.Delete(companyCandidate);
        }

        #endregion

        #region CompanyCandidate

        public CompanyCandidate GetCompanyCandidateById(int id)
        {
            if (id == 0)
                return null;

            return _companyCandidateRepository.GetById(id);
        }

        public IList<CompanyCandidate> GetCompanyCandidatesByCompanyIdAndCandidateId(int companyId, int candidateId, DateTime? refDate = null, bool postdated = false)
        {
            if (companyId == 0 || candidateId == 0)
                return null;

            var query = _companyCandidateRepository.Table;
            if (refDate.HasValue)
            {
                query = query.Where(x => !x.EndDate.HasValue || x.EndDate >= refDate);
                if (postdated == false)
                    query = query.Where(x => x.StartDate <= refDate.Value);
            }

            query = query.Where(x => x.CompanyId == companyId && x.CandidateId == candidateId).OrderBy(x => x.StartDate);

            return query.ToList();
        }

        public void UpdateCompanyCandidateRatingValue(int companyCandidateId, decimal rating)
        {
            var entry = _companyCandidateRepository.GetById(companyCandidateId);
            if (entry != null)
            {
                entry.RatingValue = (int)(Math.Round(rating, 0));
                entry.RatedBy = _workContext.CurrentAccount.FirstName + ' ' + _workContext.CurrentAccount.LastName;
                this.UpdateCompanyCandidate(entry);
            }
        }
        public void AddCandidatesToCompanyIfNotYet(int companyId, int[] employeeIds, DateTime startDate)
        {
            var notToDoList = _companyCandidateRepository.Table
                .Where(x => x.CompanyId == companyId && employeeIds.Contains(x.CandidateId))
                .Select(x => x.CandidateId).ToArray();
            foreach (var id in employeeIds.Except(notToDoList))
            {
                InsertCompanyCandidate(new CompanyCandidate()
                {
                    CandidateId = id,
                    CompanyId = companyId,
                    StartDate = startDate,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                });
            }
        }
        #endregion


        #region LIST

        public IQueryable<CompanyCandidatePoolVM> GetAvailablesFromCompanyCandidatePool(JobOrder jobOrder, DateTime refDate, bool includePlacedInOtherCompanies)
        {
            var prevDate = refDate.AddDays(-1);
            var nextDate = refDate.AddDays(1);
            var dates = new List<DateTime>() { prevDate, refDate, nextDate };
            var minHours = _settingService.GetSettingByKey<int>("CandidatePlacement.MinHoursBetweenTwoShifts");

            var shiftStart = refDate + jobOrder.StartTime.TimeOfDay;
            var shiftEnd = jobOrder.EndTime.TimeOfDay > jobOrder.StartTime.TimeOfDay ? 
                refDate + jobOrder.EndTime.TimeOfDay : nextDate + jobOrder.EndTime.TimeOfDay;

            var placements = _placements.TableNoTracking.Where(x => x.StartDate <= refDate && (!x.EndDate.HasValue || x.EndDate >= refDate));
            // placement day by day
            var placed = (from p in placements
                          join d in dates on 1 equals 1
                          where p.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                             p.StartDate <= d && (!p.EndDate.HasValue || p.EndDate >= d)
                          select new
                          {
                              CandidateId = p.CandidateId,
                              CompanyId = p.JobOrder.CompanyId,
                              StartDateTime = DbFunctions.CreateDateTime(d.Year, d.Month, d.Day, p.JobOrder.StartTime.Hour, p.JobOrder.StartTime.Minute, p.JobOrder.StartTime.Second),
                              EndDateTime = p.JobOrder.EndTime > p.JobOrder.StartTime ?
                                     DbFunctions.CreateDateTime(d.Year, d.Month, d.Day, p.JobOrder.EndTime.Hour, p.JobOrder.EndTime.Minute, p.JobOrder.EndTime.Second) :
                                     DbFunctions.AddHours(DbFunctions.CreateDateTime(d.Year, d.Month, d.Day, p.JobOrder.EndTime.Hour, p.JobOrder.EndTime.Minute, p.JobOrder.EndTime.Second), 24)
                          }).Distinct();

            // everyone from Company Candidate pool
            var pool = _companyCandidateRepository.TableNoTracking.Where(x => x.CompanyId == jobOrder.CompanyId);
            var available = (from p in pool
                             from t in _employeeTypes.TableNoTracking.Where(t => !_commonSettings.DisplayVendor || 
                                p.Candidate.EmployeeTypeId == null || t.Id == p.Candidate.EmployeeTypeId).DefaultIfEmpty()
                             where p.CompanyId == jobOrder.CompanyId && 
                                p.StartDate <= refDate && (p.EndDate == null || p.EndDate >= refDate) &&
                                p.Candidate.FranchiseId == jobOrder.FranchiseId && !p.Candidate.UseForDirectPlacement &&
                                t.Code != "REG"
                             select p.CandidateId)
                             .Distinct();

            // except anyone who is PLACED in any job order on the same day and the shifts would overlap
            // except anyone who is PLACED in any job order on the same day to another Company
            var overlapped = placed.Where(x => x.StartDateTime <= shiftEnd && DbFunctions.AddHours(x.EndDateTime, minHours) >= shiftStart)
                .Where(x => !includePlacedInOtherCompanies || x.CompanyId == jobOrder.CompanyId)
                .Select(x => x.CandidateId).Distinct();
            available = available.Except(overlapped);

            // except anyone who is in the job order pool for the day (placed and standby)
            var inPipeline = placements.Where(x => x.JobOrderId == jobOrder.Id).Select(x => x.CandidateId).Distinct();
            available = available.Except(inPipeline);

            // availability set by candidates
            var availability = _availabilityService.GetAllCandidateAvailability(refDate, refDate);

            var hours = _candidateWorkTimeRepository.TableNoTracking.Where(x => x.CompanyId == jobOrder.CompanyId)
                .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched &&
                            x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Rejected);
            var totalHours = hours.GroupBy(x => x.CandidateId).Select(g => new
            {
                CandidateId = g.Key,
                TotalWorkingHours = g.Sum(x => x.NetWorkTimeInHours)
            });

            var approvedHours = hours.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved);
            var workRecords = from ah in approvedHours
                              from l in _companyLocationRepository.TableNoTracking.Where(l => l.Id == ah.CompanyLocationId).DefaultIfEmpty()
                              from s in _shifts.TableNoTracking.Where(s => s.Id == ah.ShiftId).DefaultIfEmpty()
                              select new
                              {
                                  CandidateId = ah.CandidateId,
                                  WorkingDate = ah.JobEndDateTime,
                                  WorkingLocation = l != null ? l.LocationName : string.Empty,
                                  WorkingShift = s != null ? s.ShiftName : string.Empty,
                              };

            var lastWork = from th in totalHours
                           from wr in workRecords.Where(x => x.CandidateId == th.CandidateId).OrderByDescending(x => x.WorkingDate).Take(1)
                           select new
                           {
                               CandidateId = th.CandidateId,
                               LastWorkingDate = wr.WorkingDate,
                               LastWorkingLocation = wr.WorkingLocation,
                               LastWorkingShift = wr.WorkingShift,
                               TotalWorkingHours = th.TotalWorkingHours
                           };

            var result = from a in available
                         join p in pool on a equals p.CandidateId
                         from av in availability.Where(av => av.CandidateId == p.CandidateId).DefaultIfEmpty()
                         from lw in lastWork.Where(lw => lw.CandidateId == a).DefaultIfEmpty()
                         where p.Candidate.FranchiseId == jobOrder.FranchiseId && 
                             p.Candidate.IsActive && !p.Candidate.IsDeleted && !p.Candidate.IsBanned &&
                             (p.Candidate.IsEmployee ||
                              p.Candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started ||
                              p.Candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished)
                         select new CompanyCandidatePoolVM()
                         {
                             CandidateId = a,
                             CompanyCandidateId = p.Id,
                             CandidateGuid = p.Candidate.CandidateGuid,
                             EmployeeId = p.Candidate.EmployeeId,
                             FranchiseId = p.Candidate.FranchiseId,
                             FirstName = p.Candidate.FirstName,
                             LastName = p.Candidate.LastName,
                             AvailableShiftId = av != null ? av.ShiftId : 0,
                             RatingValue = p.RatingValue,
                             HomePhone = p.Candidate.HomePhone,
                             MobilePhone = p.Candidate.MobilePhone,
                             Email = p.Candidate.Email,
                             Note = p.Note,
                             GenderId = p.Candidate.GenderId,
                             TransportationId = p.Candidate.TransportationId,
                             Position = p.Position,
                             ShiftId = p.Candidate.ShiftId,
                             MajorIntersection = p.Candidate.MajorIntersection1 + "/" + p.Candidate.MajorIntersection2,
                             PreferredWorkLocation = p.Candidate.PreferredWorkLocation,
                             LastWorkingDate = lw != null ? lw.LastWorkingDate : (DateTime?)null,
                             LastWorkingLocation = lw != null ? lw.LastWorkingLocation : null,
                             LastWorkingShift = lw != null ? lw.LastWorkingShift : null,
                             TotalWorkingHours = lw != null ? lw.TotalWorkingHours : (decimal?)null,
                         };

            return result;
        }


        public IList<CompanyCandidate> GetCompanyCandidatePool(int companyId, DateTime? refDate = null, bool postdated = false)
        {
            return GetCompanyCandidatesByCompanyIdAsQueryable(companyId, refDate, postdated).ToList();
        }

        public IQueryable<CompanyCandidate> GetCompanyCandidatesByCompanyIdAsQueryable(int companyId, DateTime? refDate = null, bool postdated = false)
        {
            if (companyId == 0)
                return null;

            var query = _companyCandidateRepository.Table;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);


            if (refDate.HasValue)
            {
                query = query.Where(x => !x.EndDate.HasValue || x.EndDate >= refDate);
                if (postdated == false)
                    query = query.Where(x => x.StartDate <= refDate.Value);
            }

            query = query.Include(x => x.Candidate)
                        .Where(x => x.CompanyId == companyId
                                    && x.Candidate.IsActive
                                    && !x.Candidate.IsBanned
                                    && !x.Candidate.IsDeleted)
                        .OrderBy(x => x.CandidateId).Select(x => x);

            return query;
        }

        public IQueryable<CandidateWithAddress> GetAllCandidatesForCompanyPoolAsQueryable(Account account, int companyId, bool showInactive = false, bool showBanned = false, bool showHidden = false)
        {
            var query = _candidateRepository.TableNoTracking;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // banned
            if (!showBanned)
                query = query.Where(c => c.IsBanned == false);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = query.Where(c => !c.UseForDirectPlacement);

            if (account.IsLimitedToFranchises)
                query = query.Where(c => c.FranchiseId == account.FranchiseId);

            //query = query.Where(c => c.IsEmployee || c.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started ||
            //                    c.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished);

           /// query = query.Where(c => c.EmployeeTypeId == null || c.EmployeeTypeId.Value != 3);

            var result = query.Where(x => !_companyCandidateRepository.Table.Where(c => c.CompanyId == companyId).Select(c => c.CandidateId).Contains(x.Id))
                    .Select(c => new CandidateWithAddress
                    {
                        Guid = c.CandidateGuid,
                        CityId = c.CandidateAddresses.Where(ca => ca.AddressTypeId == (int)AddressTypeEnum.Residential).Select(ca => ca.CityId).FirstOrDefault(),
                        StateProvinceId = c.CandidateAddresses.Where(ca => ca.AddressTypeId == (int)AddressTypeEnum.Residential).Select(ca => ca.StateProvinceId).FirstOrDefault(),
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        GenderId = c.GenderId,
                        Email = c.Email,
                        SearchKeys = c.SearchKeys,
                        HomePhone = c.HomePhone,
                        MobilePhone = c.MobilePhone,
                        EmergencyPhone = c.EmergencyPhone,
                        ShiftId = c.ShiftId,
                        TransportationId = c.TransportationId,
                        FranchiseId = c.FranchiseId,
                        UpdatedOnUtc = c.UpdatedOnUtc,
                        CreatedOnUtc = c.CreatedOnUtc,
                        IsActive=c.IsActive,
                        IsBanned=c.IsBanned,
                        IsDeleted=c.IsDeleted,
                        EmployeeId = c.EmployeeId
                    }).OrderByDescending(c=>c.UpdatedOnUtc ).AsQueryable();

            return result;
        }


        public IQueryable<CompanyCandidate> GetAllCompanyCandidatesAsQueryable(Account account)
        {
            var query = _companyCandidateRepository.Table;

            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(x => x.Candidate.FranchiseId == account.FranchiseId);

            return query;
        }


        public IQueryable<CompanyCandidate> GetCompanyCandidatesByAccountAndCompany(Account account, int companyId, DateTime refDate)
        {
            var candidates = GetCompanyCandidatesByCompanyIdAsQueryable(companyId, refDate);
            if (account.IsLimitedToFranchises)
                candidates = candidates.Where(x => x.Candidate.FranchiseId == account.FranchiseId);

            return candidates.Where(x => x.Candidate.IsEmployee ||
                                    x.Candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started ||
                                    x.Candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished);
        }


        //public void GetAdditionalInfo(CompanyCandidate entry, ICompanyCandidatePriorityInfo priorityInfo)
        //{
        //    var query = _candidateWorkTimeRepository.Table;
        //    if (_commonSettings.DisplayVendor)
        //        query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);

        //    var allData = query.Where(x=>x.CandidateId == entry.CandidateId &&
        //                    x.CompanyId == entry.CompanyId &&
        //                    x.CandidateWorkTimeStatusId == (int)(CandidateWorkTimeStatus.Approved))
        //        .OrderByDescending(x => x.JobEndDateTime).ToArray();

        //    priorityInfo.LastWorkingDate = allData.Any() ? new DateTime?(allData[0].JobEndDateTime) : null;
        //    priorityInfo.LastWorkingShift = allData.Any() ? allData[0].JobShift : null;
        //    priorityInfo.LastWorkingLocation = allData.Any() ? _companyLocationRepository.TableNoTracking.Where(x => x.Id == allData[0].CompanyLocationId).FirstOrDefault().LocationName : null;
        //    priorityInfo.TotalWorkingHours = allData.Sum(x => x.NetWorkTimeInHours);
        //}

        #endregion


        #region remove candidate

        public string RemoveCandidateFromPool(CompanyCandidate companyCandidate, string reason, DateTime startDate)
        {
            var result = String.Empty;

            if (!String.IsNullOrEmpty(reason) && startDate > companyCandidate.StartDate)
            {
                companyCandidate.EndDate = startDate.AddDays(-1);
                companyCandidate.ReasonForLeave = reason;
                companyCandidate.Note = companyCandidate.Note + " --Banned--";

                companyCandidate.UpdatedOnUtc = DateTime.UtcNow;
                UpdateCompanyCandidate(companyCandidate);
            }
            else
                result = "Reason or date not given.";

            return result;
        }


        public string RemoveCandidateFromAllPools(int candidateId, string reason, DateTime startDate, int? companyId = null)
        {
            var result = String.Empty;

            var pools = _companyCandidateRepository.Table
                        .Where(x => x.CandidateId == candidateId)
                        .Where(x => !x.EndDate.HasValue || x.EndDate >= startDate);

            if (companyId.HasValue)
                pools = pools.Where(x => x.CompanyId == companyId);

            foreach (var p in pools.ToList())
            {
                if (p.StartDate >= startDate)
                    DeleteCompanyCandidate(p);
                else
                {
                    var msg = RemoveCandidateFromPool(p, reason, startDate);
                    if (!String.IsNullOrEmpty(msg))
                        result += msg + "\r\n";
                }
            }

            return result;
        }

        #endregion

    }
}
