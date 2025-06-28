using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Services.Candidates;

namespace Wfm.Admin.Models.ClockTime
{
    public class CandidateSmartCardModel_BL
    {
        #region Fields
        private readonly ICandidateService _candidateService;
        #endregion

        #region Ctor
        public CandidateSmartCardModel_BL(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }
        #endregion
        public CandidateSmartCardModel GetNewSmardCardModel(Guid? guid,out string error)
        {
            error = string.Empty;
            var caniddate = _candidateService.GetCandidateByGuid(guid);
            if (caniddate == null)
            {
                error = "The candidate does not exist!";
                return null;
            }
            var model = new CandidateSmartCardModel();

            model.CandidateId = caniddate.Id;

            model.CandidateFirstName = caniddate.FirstName;
            model.CandidateLastName = caniddate.LastName;
            model.CandidateGuid = caniddate.CandidateGuid;
            model.EmployeeId = caniddate.EmployeeId;
            model.IsActive = true;
            model.ActivatedDate = System.DateTime.UtcNow;

            model.UpdatedOnUtc = System.DateTime.UtcNow;
            model.CreatedOnUtc = System.DateTime.UtcNow;
            return model;
        }
    }
}