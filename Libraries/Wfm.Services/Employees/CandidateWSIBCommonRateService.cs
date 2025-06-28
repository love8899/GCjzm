using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Data;
using Wfm.Core.Domain.WSIB;

namespace Wfm.Services.Employees
{
    public class CandidateWSIBCommonRateService:ICandidateWSIBCommonRateService
    {
        #region Fields
        private readonly IRepository<CandidateWSIBCommonRate> _candidateWSIBCommonRateRepository;
        #endregion

        #region Ctor
        public CandidateWSIBCommonRateService(IRepository<CandidateWSIBCommonRate> candidateWSIBCommonRateRepository)
        {
            _candidateWSIBCommonRateRepository = candidateWSIBCommonRateRepository;
        }
        #endregion

        #region CRUD
        public void Create(CandidateWSIBCommonRate entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            entity.CreatedOnUtc = entity.UpdatedOnUtc = DateTime.UtcNow;
            _candidateWSIBCommonRateRepository.Insert(entity);
        }

        public CandidateWSIBCommonRate Retrieve(int id)
        {
            if (id == 0)
                return null;
            return _candidateWSIBCommonRateRepository.GetById(id);
        }

        public void Update(CandidateWSIBCommonRate entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _candidateWSIBCommonRateRepository.Update(entity);
        }

        public void Delete(CandidateWSIBCommonRate entity)
        {
            if (entity == null)
                throw new ArgumentNullException();

            _candidateWSIBCommonRateRepository.Delete(entity);
        }

        #endregion

        #region Methods
        public IQueryable<CandidateWSIBCommonRate> GetAllWSIBCommonRateByCandidateId(int candidate)
        {
            var result = _candidateWSIBCommonRateRepository.Table.Where(x => x.CandidateId == candidate);
            return result;
        }

        public bool ValidateWSIBCommonRates(List<CandidateWSIBCommonRate> rates,out string errors)
        {
            errors = string.Empty;
            if (rates.Count <= 0)
                return true;
            else
            {
                StringBuilder error = new StringBuilder();

                //check the sum of ratio should be 100% for the same period
                var groups = rates.GroupBy(x => new { x.StartDate, x.EndDate }).OrderBy(x => x.Key.StartDate);

                foreach (var group in groups)
                {
                    foreach (var group2 in groups.Where(x => x.Key != group.Key && x.Key.StartDate > group.Key.StartDate))
                    {
                        if (group.Key.StartDate <= group2.Key.EndDate && group.Key.EndDate >= group2.Key.StartDate)
                            error.AppendLine(String.Format("The data range from {0} to {1} overlaps with the one from {2} to {3}!", group.Key.StartDate.ToShortDateString(), group.Key.EndDate.ToShortDateString(), group2.Key.StartDate.ToShortDateString(), group2.Key.EndDate.ToShortDateString()));
                    }
                    decimal sum = 0.00M;
                    foreach (var rate in group)
                    {
                        sum += rate.Ratio;
                    }
                    if (sum == 1.00M)
                        continue;
                    else
                        error.AppendLine(String.Format("The sum of ratio betweeen {0} and {1} must be 100%!", group.Key.StartDate.ToShortDateString(), group.Key.EndDate.ToShortDateString()));
                }

                if (error.Length <= 0)
                    return true;
                else
                {
                    errors = error.ToString();
                    return false;
                }
            }
        }
        #endregion
 
    }
}
