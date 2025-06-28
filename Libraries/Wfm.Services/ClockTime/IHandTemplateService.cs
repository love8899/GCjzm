using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Services.ClockTime
{
    public partial interface IHandTemplateService
    {

        #region CRUD

        void Insert(HandTemplate handTemplate);

        void Update(HandTemplate handTemplate);

        void Delete(HandTemplate handTemplate);

        void InsertOrUpdate(HandTemplate handTemplate);

        #endregion


        #region HandTemplate

        HandTemplate GetHandTemplateById(int id);

        HandTemplate GetHandTemplateByGuid(Guid guid);

        HandTemplate GetActiveHandTemplateByCandidateId(int candidateId);

        #endregion


        #region List

        IList<HandTemplate> GetAllHandTemplatesByCandidateId(int candidateId, bool showInactive = false);

        IQueryable<HandTemplate> GetAllHandTemplatesAsQueryable(bool showHidden = false, bool showInactive = true);

        #endregion

    }
}
