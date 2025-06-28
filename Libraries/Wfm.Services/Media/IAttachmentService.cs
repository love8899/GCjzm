using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Media
{
    public partial interface IAttachmentService
    {
        #region CRUD

        void InsertAttachment(CandidateAttachment attachment);

        void UpdateAttachment(CandidateAttachment attachment);

        void DeleteAttachment(CandidateAttachment attachment);


        string UploadCandidateAttachment(int candidateId, byte[] attachmentBinary, string fileName, string contentType, int documentTypeId,DateTime? expiryDate=null,int? companyId=null);

        #endregion

        #region Attachment

        CandidateAttachment GetAttachmentById(int id, bool noTracking = true);

        CandidateAttachment GetAttachmentByGuid(Guid? guid, bool noTracking = true);

        CandidateAttachment GetCandidateResumeByCandidateGuid(Guid? guid, bool noTracking = true);

        #endregion

        #region LIST

        List<CandidateAttachment> GetAttachmentsByCandidateId(int candidateId,bool isPublic=false);

        IQueryable<CandidateAttachment> GetAllCandidateAttachmentsAsQueryable(bool showInactive = false, bool showHidden = false);

        #endregion

        #region Helpers

        string GetMimeType(string fileName);

        void ReloadContentTextById(int id);

        string ExtractFileText(byte[] fileBinary, string fileExtension);

        #endregion

    }
}
