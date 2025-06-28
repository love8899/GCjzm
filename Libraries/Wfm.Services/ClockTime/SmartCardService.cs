using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core;

namespace Wfm.Services.ClockTime
{
    public class SmartCardService : ISmartCardService
    {
        #region Fields

        IRepository<CandidateSmartCard> _smartCardRepository;
        IRepository<Candidate> _candidateRepository;
        IRepository<CandidateJobOrder> _placementRepository;
        private readonly CommonSettings _commonSettings;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public SmartCardService(IRepository<CandidateSmartCard> smartCardRepository,
            IRepository<Candidate> candidateRepository,
            IRepository<CandidateJobOrder> placementRepository,
            CommonSettings commonSettings,
            IWorkContext workContext)
        {
            _smartCardRepository = smartCardRepository;
            _candidateRepository = candidateRepository;
            _placementRepository = placementRepository;
            _commonSettings = commonSettings;
            _workContext = workContext;
        }

        #endregion

        #region CRUD

        public void Insert(CandidateSmartCard smartCard)
        {
            if (smartCard == null)
                throw new ArgumentNullException("smartCard");

            smartCard.CreatedOnUtc = DateTime.UtcNow;
            smartCard.UpdatedOnUtc = DateTime.UtcNow;

            _smartCardRepository.Insert(smartCard);
        }

        public void Update(CandidateSmartCard smartCard)
        {
            if (smartCard == null)
                throw new ArgumentNullException("smartCard");
            smartCard.UpdatedOnUtc = DateTime.UtcNow;

            _smartCardRepository.Update(smartCard);
        }

        public virtual void Delete(CandidateSmartCard smartCard)
        {
            if (smartCard == null)
                throw new ArgumentNullException("smartCard");

            smartCard.UpdatedOnUtc = DateTime.UtcNow;

            _smartCardRepository.Delete(smartCard);
        }

        public void InsertDefaultSmartCard(string smartCardUid, int candidateId)
        {
            var card = new CandidateSmartCard();
            card.CandidateId = candidateId;
            card.SmartCardUid = smartCardUid;
            card.IsActive = true;
            card.IsDeleted = false;
            card.ActivatedDate = DateTime.Today;
            card.EnteredBy = _workContext.CurrentAccount.Id;
            Insert(card);

        }
        #endregion

        #region SmartCard

        public CandidateSmartCard GetSmartCardById(int id)
        {
            if (id <= 0)
                return null;
           
            var query = _smartCardRepository.Table;
            
            query = from c in query
                    where c.Id == id
                    select c;

            return query.FirstOrDefault();
        }


        public bool IsDuplicate(string smartCardUid, int? id = null)
        {
            if (string.IsNullOrWhiteSpace(smartCardUid))
                return true;

            var query = _smartCardRepository.TableNoTracking
                .Where(x => x.SmartCardUid == smartCardUid)
                .Where(x => !id.HasValue || x.Id != id);

            return query.Any();
        }


        public CandidateSmartCard GetSmartCardBySmartCardUid(string smartCardUid)
        {
            if (string.IsNullOrWhiteSpace(smartCardUid))
                return null;

            var query = _smartCardRepository.Table;
            
            query = from c in query
                    where c.SmartCardUid == smartCardUid
                    select c;

            return query.FirstOrDefault();
        }

        public CandidateSmartCard GetActiveSmartCardByCandidateId(int candidateId)
        {
            if (candidateId <= 0)
                return null;

            var query = _smartCardRepository.Table;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = from c in _smartCardRepository.Table
                    where c.CandidateId == candidateId && c.IsActive == true
                    orderby c.UpdatedOnUtc descending
                    select c;

            return query.FirstOrDefault();
        }


        public Candidate GetCandidateBySmartCardUid(string smartCardUid, bool activeOnly = true, DateTime? refDate = null)
        {
            if (string.IsNullOrWhiteSpace(smartCardUid))
                return null;

            var query = _candidateRepository.Table;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.EmployeeTypeId != (int)EmployeeTypeEnum.REG);

            query = from c in query
                    join s in _smartCardRepository.Table on c.Id equals s.CandidateId
                    where s.SmartCardUid == smartCardUid && 
                          (!activeOnly || (s.IsActive && !s.IsDeleted && (!refDate.HasValue || !s.ActivatedDate.HasValue || s.ActivatedDate <= refDate)))
                          
                    select c;

            return query.FirstOrDefault();
        }


        public IList<CandidateSmartCard> GetSmartCardsByPartialSmartCardUid(string smartCardUid)
        {
            if (string.IsNullOrWhiteSpace(smartCardUid))
                return null;

            var query = _smartCardRepository.TableNoTracking;
            query = query.Where(x => x.IsActive && x.SmartCardUid.Contains(smartCardUid));

            return query.ToList();
        }


        public CandidateSmartCard GetSmartCardByGuid(Guid guid) 
        {
            if (guid == null)
                return null;

            var query = _smartCardRepository.Table;
            
            query = from c in query
                    where c.CandidateSmartCardGuid == guid
                    select c;

            return query.FirstOrDefault();
        }


        public string GetIdString(CandidateSmartCard smartCard, int? idLength)
        {
            var idStr = smartCard.SmartCardUid;

            if (!idLength.HasValue || idLength == 10)
                ;

            else if (idLength == 8)
                idStr = (smartCard.FacilityCode * 100000 + smartCard.CardNumber).ToString();

            else if (idLength == 5)
                idStr = smartCard.CardNumber.ToString();

            return idStr;
        }


        public Candidate GetCandidateByIdString(CompanyClockDevice clockDevice, string idString, out string smartCardUid, bool activeOnly = true, DateTime? refDate = null)
        {
            Candidate candidate = null;
            smartCardUid = idString;

            if (string.IsNullOrWhiteSpace(idString))
                return candidate;

            if (!clockDevice.IDLength.HasValue || clockDevice.IDLength == 10)
                return GetCandidateBySmartCardUid(smartCardUid);

            else
            {
                var candidates = GetCandidatesByIdString(clockDevice.CompanyLocation.CompanyId, idString, out smartCardUid, activeOnly, refDate);
                try
                {
                    candidate = candidates.SingleOrDefault();
                }
                catch (Exception exc)   // none or duplicate
                {
                    ;
                }

                return candidate;
            }
        }


        public IEnumerable<Candidate> GetCandidatesByIdString(int companyId, string idString, out string smartCardUid, bool activeOnly = true, DateTime? refDate = null)
        {
            smartCardUid = idString;
            var candidates = Enumerable.Empty<Candidate>();
            var id = Convert.ToInt32(idString);
            var facilityCode = id / 100000;
            var cardNumber = id % 100000;
            var cards = GetAllSmartCardsAsQueryable(showInactive: !activeOnly)
                .Where(x => !refDate.HasValue || !x.ActivatedDate.HasValue || x.ActivatedDate <= refDate)
                .Where(x => (facilityCode == 0 || x.FacilityCode == facilityCode) && x.CardNumber == cardNumber);

            if (cards.Any())
            {
                try
                {
                    var card = cards.SingleOrDefault();
                    if (card != null)   // no duplicate cards globally
                    {
                        smartCardUid = card.SmartCardUid;
                        candidates = cards.Select(x => x.Candidate);
                    }
                }
                catch (InvalidOperationException exc0)   // duplicate cards globally
                {
                    var prevDay = refDate.HasValue ? refDate.Value.AddDays(-1) : (DateTime?)null;
                    var nextDay = refDate.HasValue ? refDate.Value.AddDays(1) : (DateTime?)null;
                    var placed = _placementRepository.Table.Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed)
                        .Where(x => !refDate.HasValue || (x.StartDate <= nextDay && (!x.EndDate.HasValue || x.EndDate > prevDay)))
                        .Where(x => x.JobOrder.CompanyId == companyId).Select(x => new { x.CandidateId }).Distinct();

                    candidates = from c in cards
                                 join p in placed on c.CandidateId equals p.CandidateId
                                 select c.Candidate;
                    try
                    {
                        var candidate = candidates.SingleOrDefault();
                        if (candidate != null)  // no duplicate cards within company
                            smartCardUid = cards.First(x => x.CandidateId == candidate.Id).SmartCardUid;
                    }
                    catch (InvalidOperationException exc1)   // duplicate cards within company
                    {
                        ;
                    }
                }
            }

            return candidates;
        }

        #endregion


        #region Paged List

        public IList<CandidateSmartCard> GetAllSmartCards()
        {
            var query = _smartCardRepository.TableNoTracking;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = from s in query
                    where s.IsActive == true
                    orderby s.CreatedOnUtc descending
                    select s;

            return query.ToList();
        }

        public IList<CandidateSmartCard> GetAllSmartCardsByCandidateId(int candidateId, bool showInactive = false)
        {
            if (candidateId == 0)
                return null;

            var query = _smartCardRepository.TableNoTracking;

            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);

            query = from s in query
                    where s.CandidateId == candidateId
                    orderby s.CreatedOnUtc descending
                    select s;

            return query.ToList();
        }

        public IQueryable<CandidateSmartCard> GetAllSmartCardsAsQueryable(bool showHidden = false, bool showInactive = true)
        {
            var query = _smartCardRepository.TableNoTracking;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            if (!showHidden)
                query = query.Where(c => !c.IsDeleted);

            query = query.Where(c => showInactive || c.IsActive);

            query = from c in query
                    orderby c.UpdatedOnUtc descending, c.CandidateId descending
                    select c;

            return query.AsQueryable();
        }

        #endregion

    }
}
