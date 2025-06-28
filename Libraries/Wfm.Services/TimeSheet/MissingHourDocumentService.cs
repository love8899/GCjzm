using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Services.TimeSheet
{
    public partial class MissingHourDocumentService : IMissingHourDocumentService
    {
        #region Fields

        private readonly IRepository<MissingHourDocument> _missingHourDocRepository;

        #endregion


        #region Ctor

        public MissingHourDocumentService(IRepository<MissingHourDocument> missingHourDocRepository)
        {
            _missingHourDocRepository = missingHourDocRepository;
        }

        #endregion


        public void InsertMissingHourDocument(MissingHourDocument missingHourDoc)
        {
            if (missingHourDoc == null)
                throw new ArgumentNullException("missingHourDoc");

            _missingHourDocRepository.Insert(missingHourDoc);
        }


        public void DeleteMissingHourDocument(MissingHourDocument missingHourDoc)
        {
            if (missingHourDoc == null)
                throw new ArgumentNullException("missingHourDoc");

            _missingHourDocRepository.Delete(missingHourDoc);
        }


        public MissingHourDocument GetMissingHourDocumentById(int id)
        {
            if (id == 0)
                return null;

            return _missingHourDocRepository.GetById(id);
        }


        public IQueryable<MissingHourDocument> GetAllMissingHourDocumentsByMissingHourId(int missingHourId)
        {
            var query = _missingHourDocRepository.TableNoTracking;

            query = query.Where(x => x.CandidateMissingHourId == missingHourId);

            return query;
        }

    }
}
