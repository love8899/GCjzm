using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    /// <summary>
    /// CandidatePicture service interface
    /// </summary>
    public partial interface ICandidatePictureService
    {
        #region CRUD

        void InsertCandidatePicture(CandidatePicture candidatePicture);

        void UpdateCandidatePicture(CandidatePicture candidatePicture);

        void DeleteCandidatePicture(CandidatePicture candidatePicture);

        CandidatePicture InsertCandidatePicture(byte[] candidatePictureBinary, string mimeType, int candidateId,
            bool isNew, int displayOrder = 0, bool validateBinary = true);

        CandidatePicture UpdateCandidatePicture(int id, byte[] candidatePictureBinary, string mimeType, bool isNew,
            bool validateBinary = true);

        byte[] ValidateCandidatePicture(byte[] candidatePictureBinary, string mimeType);

        #endregion


        #region CandidatePicture

        CandidatePicture GetCandidatePictureById(int id);

        #endregion


        #region #region Getting candidatePicture local path/URL methods

        byte[] LoadCandidatePictureBinary(CandidatePicture candidatePicture);

        string GetDefaultCandidatePictureUrl(int targetSize = 0,
            CandidatePictureType defaultCandidatePictureType = CandidatePictureType.Entity,
            string franchiseLocation = null);


        string GetCandidatePictureUrl(int candidatePictureId,
            int targetSize = 0,
            bool showDefaultCandidatePicture = true,
            string franchiseLocation = null,
            CandidatePictureType defaultCandidatePictureType = CandidatePictureType.Entity);


        string GetCandidatePictureUrl(CandidatePicture candidatePicture,
            int targetSize = 0,
            bool showDefaultCandidatePicture = true,
            string franchiseLocation = null,
            CandidatePictureType defaultCandidatePictureType = CandidatePictureType.Entity);


        string GetThumbLocalPath(CandidatePicture candidatePicture, int targetSize = 0, bool showDefaultCandidatePicture = true);

        #endregion


        #region LIST

        IList<CandidatePicture> GetCandidatePicturesByCandidateId(int candidateId, int recordsToReturn = 0);

        #endregion

    }
}
