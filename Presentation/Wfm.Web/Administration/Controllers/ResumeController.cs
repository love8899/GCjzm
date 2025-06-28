using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Messages;
using Wfm.Core;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Web.Framework;


namespace Wfm.Admin.Controllers
{
    public class ResumeController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IResumeService _resumeService;
        private readonly IResumeHistoryService _resumeHistoryService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;

        #endregion


        #region Ctor

        public ResumeController(IPermissionService permissionService,
                              IResumeService resumeService,
                              IResumeHistoryService resumeHistoryService,
                              IWorkContext workContext,
                              IWebHelper webHelper
               )
        {
            _permissionService = permissionService;
            _resumeService = resumeService;
            _resumeHistoryService = resumeHistoryService;
            _workContext = workContext;
            _webHelper = webHelper;
        }

        #endregion


        #region Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (!(_workContext.CurrentFranchise.IsDefaultManagedServiceProvider ||
                  _workContext.CurrentFranchise.FranchiseName.StartsWith("Great Connections")))
                return AccessDeniedView();

            return View();
        }


        public ActionResult _MessageGrid()
        {
            ViewBag.FilterDescriptors = (IList<IFilterDescriptor>)Session["FilterDescriptors"];

            return PartialView();
        }


        [HttpPost]
        public ActionResult _MessageGrid([DataSourceRequest] DataSourceRequest request, ResumeCriteria criteria)
        {
            Session["FilterDescriptors"] = request.Filters;
            //Session["SortDescriptors"] = request.Sorts;

            var messages = _resumeService.GetAllResumesByCriteria(criteria);

            // filter by contact status
            var contactedFilter = request.ExtractFilterValue("IsContacted");
            if (!string.IsNullOrWhiteSpace(contactedFilter))
            {
                messages = contactedFilter == "True" ?
                    messages.Where(x => x.ResumeHistories.Any()) :
                    messages.Where(x => !x.ResumeHistories.Any());
            }

            var result = messages.ToDataSourceResult(request, x => x.ToModel());

            return Json(result);
        }

        #endregion


        #region Message Details

        public ActionResult _MessageDetails(int id, ResumeCriteria criteria)
        {
            var message = _resumeService.GetResumeById(id);

            int prevMsgId, nextMsgId, currIndex, total;
            _GetPreviousAndNextMessageIds(id, criteria, out prevMsgId, out nextMsgId, out currIndex, out total);
            ViewBag.PrevMsgId = prevMsgId;
            ViewBag.CurrMsgId = id;
            ViewBag.NextmsgId = nextMsgId;
            ViewBag.CurrIndex = currIndex;
            ViewBag.Total = total;

            var model = message.ToModel();
            if (!message.IsRead)
            {
                message.IsRead = true;
                _resumeService.UpdateResume(message);
            }

            return PartialView(model);
        }


        public void _GetPreviousAndNextMessages(int id, ResumeCriteria criteria, out Resume prevMsg, out Resume nextMsg, out int currIndex, out int total)
        {
            var messages = _resumeService.GetAllResumesByCriteria(criteria);
            // TODO: sort descriptors
            var filtered = messages;
            var filterDescriptors = (IList<IFilterDescriptor>)Session["FilterDescriptors"];
            if (filterDescriptors != null && filterDescriptors.Any())
                filtered = messages.Where(filterDescriptors) as IQueryable<Resume>;
            var before = filtered.Where(x => x.Id < id);
            var after = filtered.Where(x => x.Id > id);
            currIndex = before.Count();
            prevMsg = before.OrderByDescending(x => x.Id).FirstOrDefault();
            nextMsg = after.FirstOrDefault();
            total = filtered.Count();
        }


        public void _GetPreviousAndNextMessageIds(int id, ResumeCriteria criteria, out int prevMsgId, out int nextMsgId, out int currIndex, out int total)
        {
            Resume prevMsg, nextMsg;
            _GetPreviousAndNextMessages(id, criteria, out prevMsg, out nextMsg, out currIndex, out total);

            prevMsgId = prevMsg != null ? prevMsg.Id : 0;
            nextMsgId = nextMsg != null ? nextMsg.Id : 0;
        }


        public JsonResult _GetResumeHistories([DataSourceRequest] DataSourceRequest request, int resumeId)
        {
            var histories = _resumeHistoryService.GetAllResumeHistoriesByResume(resumeId);

            return Json(histories.ToDataSourceResult(request, x => x.ToModel()));
        }


        [HttpPost]
        public ActionResult _SaveResumeHistory([DataSourceRequest] DataSourceRequest request, ResumeHistoryModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    model.ContactedOnUtc = model.ContactedOn.Date.ToUniversalTime();    // date only
                    var entity = model.Id == 0 ? new ResumeHistory() : 
                        _resumeHistoryService.GetResumeHistoryById(model.Id);

                    entity.ContactedOnUtc = model.ContactedOnUtc;
                    entity.Via = model.Via;
                    entity.Result = model.Result;
                    entity.Note = model.Note;

                    if (model.Id > 0)
                        _resumeHistoryService.UpdateResumeHistory(entity);

                    else
                    {
                        entity.ResumeId = model.ResumeId;
                        entity.AccountId = _workContext.CurrentAccount.Id;

                        _resumeHistoryService.InsertResumeHistory(entity);

                        model.Id = entity.Id;
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region Download

        public ActionResult DownloadMessageAttachment(int id)
        {
            var messsage = _resumeService.GetResumeById(id);

            if (messsage != null)
            {
                var contentType = "application/octet-stream";
                if (messsage.AttachmentBinary != null)
                    return File(messsage.AttachmentBinary, contentType, messsage.AttachmentFileName);
                else
                {
                    var filePath = Path.Combine(_webHelper.GetRootDirectory(), messsage.StoredPath, messsage.AttachmentFileName);
                    if (System.IO.File.Exists(filePath))
                        return File(filePath, contentType, messsage.AttachmentFileName);
                }
            }

            ErrorNotification("The attachment can not be found!");
            return RedirectToAction("Index");
        }


        public ActionResult DownloadMessage(int id)
        {
            var messsage = _resumeService.GetResumeById(id);

            if (messsage != null)
            {
                var contentType = "message/rfc822";
                if (messsage.EmailBinary != null)
                    return File(messsage.EmailBinary, contentType, messsage.EmailFile);
                else
                {
                    var filePath = Path.Combine(_webHelper.GetRootDirectory(), messsage.StoredPath, messsage.EmailFile);
                    if (System.IO.File.Exists(filePath))
                        return File(filePath, contentType, messsage.EmailFile);
                }
            }

            ErrorNotification("The email message can not be found!");
            return RedirectToAction("Index");
        }

        #endregion
    }
}
