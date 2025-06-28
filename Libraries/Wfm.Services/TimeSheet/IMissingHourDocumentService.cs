using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Services.TimeSheet
{
    public partial interface IMissingHourDocumentService
    {
        void InsertMissingHourDocument(MissingHourDocument missingHourDoc);

        void DeleteMissingHourDocument(MissingHourDocument missingHourDoc);

        MissingHourDocument GetMissingHourDocumentById(int id);

        IQueryable<MissingHourDocument> GetAllMissingHourDocumentsByMissingHourId(int missingHourId);
    }
}
