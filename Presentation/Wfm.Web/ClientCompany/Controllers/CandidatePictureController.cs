using System;
using System.Web.Mvc;
using Wfm.Services.Candidates;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Shared.Models.Common;

namespace Wfm.Client.Controllers
{
    public partial class CandidatePictureController : BaseClientController
    {
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly ICandidateService _candidateService;
        public CandidatePictureController(
            ICandidatePictureService candidatePictureService,
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            ICandidateService candidateService
            )
        {
            this._candidatePictureService = candidatePictureService;
            this._activityLogService = activityLogService;
            this._localizationService = localizationService;
            this._candidateService = candidateService;
        }


        #region AsyncUpload

        [HttpPost]
        public ActionResult AsyncUpload(Guid? guid)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");
            string error = string.Empty;
            var fileHelper = new FileUploadDownload();
            var picture = fileHelper.AsyncUploadCandidatePicture(guid, Request, _candidateService,_candidatePictureService,out error);
            if (error.Length > 0)
            {
                
                return Json(new { sucess = false, message = error });
            }
            else
            {
                //activity log
                _activityLogService.InsertActivityLog("AddNewCandidatePicture", _localizationService.GetResource("ActivityLog.AddNewCandidatePicture"), picture.Id + "/" + picture.CandidateId);
                return Json(new
                {
                    success = true,
                    pictureId = picture.Id,
                    imageUrl = _candidatePictureService.GetCandidatePictureUrl(picture, 100)
                },
                    "text/plain");
            }
        } 

        #endregion

    }
}
