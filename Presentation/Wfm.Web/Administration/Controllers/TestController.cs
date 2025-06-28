using System;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Wfm.Core;
using Wfm.Core.Domain.Tests;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Services.Test;
using Wfm.Admin.Models.Test;
using Wfm.Admin.Extensions;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.Extensions;
using Wfm.Services.Media;
using Wfm.Core.Domain.Media;

namespace Wfm.Admin.Controllers
{
    public class TestController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly ITestService _testService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IAttachmentTypeService _attachmentTypeService;
        #endregion

        #region Ctor

        public TestController(IActivityLogService activityLogService,
            ITestService testService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IAttachmentTypeService attachmentTypeService)
        {
            _activityLogService = activityLogService;
            _testService = testService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _attachmentTypeService = attachmentTypeService;
        }

        #endregion


        // TestCategory
        // ==============================

        #region GET :/TestCategory/Index
        [HttpGet]
        public ActionResult TestCategoryIndex()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();
            return View();
        }
        #endregion

        #region POST:/TestCategory/Index
        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            var testCategories = _testService.GetAllCategories(true);

            return Json(testCategories.ToDataSourceResult(request,m=>m.ToModel()));
        }
        #endregion

        #region GET :/TestCategory/Create

        [HttpGet]
        public ActionResult TestCategoryCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            var model = new TestCategoryModel();

            model.UpdatedOnUtc = System.DateTime.UtcNow;
            model.CreatedOnUtc = System.DateTime.UtcNow;
            model.IsActive = true;
            model.IsRequiredWhenRegistration = false;
            return View(model);
        }

        #endregion

        #region POST:/TestCategory/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult TestCategoryCreate(TestCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                TestCategory testCategory = model.ToEntity();
                testCategory.UpdatedOnUtc = System.DateTime.UtcNow;
                testCategory.CreatedOnUtc = System.DateTime.UtcNow;

                _testService.InsertTestCategory(testCategory);


                //activity log
                _activityLogService.InsertActivityLog("AddNewTestCategory", _localizationService.GetResource("ActivityLog.AddNewTestCategory"), testCategory.TestCategoryName);


                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.TestCategory.Added"));
                return continueEditing ? RedirectToAction("TestCategoryEdit", new { id = testCategory.Id }) : RedirectToAction("TestCategoryIndex");
            }

            return View(model);
        }

        #endregion

        #region GET :/TestCategory/Edit

        [HttpGet]
        public ActionResult TestCategoryEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            TestCategory testCategory = _testService.GetTestCategoryById(id);
            if (testCategory == null)
                return RedirectToAction("TestCategoryIndex");

            TestCategoryModel model = testCategory.ToModel();

            return View(model);
        }

         #endregion

        #region POST:/TestCategory/Edit
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult TestCategoryEdit(TestCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                TestCategory testCategory = _testService.GetTestCategoryById(model.Id);
                if (testCategory == null)
                    return RedirectToAction("TestCategoryIndex");

                testCategory = model.ToEntity(testCategory);
                testCategory.UpdatedOnUtc = System.DateTime.UtcNow;

                _testService.UpdateTestCategory(testCategory);


                //activity log
                _activityLogService.InsertActivityLog("UpdateTestCategory", _localizationService.GetResource("ActivityLog.UpdateTestCategory"), testCategory.TestCategoryName);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.TestCategory.Updated"));
                return continueEditing ? RedirectToAction("TestCategoryEdit", new { id = testCategory.Id }) : RedirectToAction("TestCategoryIndex");
            }

            return View(model);
        }
        #endregion


        // TestQuestion
        // ==============================

        #region GET :/TestQuestion/Index

        [HttpGet]
        public ActionResult TestQuestionIndex(int testCategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            var testQuestions = _testService.GetTestQuestionsByTestCategoryId(testCategoryId);

            var testQuestionModels = new List<TestQuestionModel>();
            foreach (var item in testQuestions)
            {
                TestQuestionModel t = item.ToModel();
                testQuestionModels.Add(t);
            }

            ViewBag.TestCategoryId = testCategoryId;

            return View(testQuestionModels);
        }

        #endregion

        #region POST:/TestQuestion/Index

        [HttpPost]
        public ActionResult TestQuestionIndex([DataSourceRequest] DataSourceRequest request, int testCategoryId)
        {
            var testQuestions = _testService.GetTestQuestionsByTestCategoryId(testCategoryId);

            var testQuestionModels = new List<TestQuestionModel>();
            foreach (var item in testQuestions)
            {
                TestQuestionModel model = item.ToModel();
                model.ImageFileUrl = _testService.GetTestImageUrl(item.ImageFileLocation);
                testQuestionModels.Add(model);
            }
            var result = new DataSourceResult()
            {
                Data = testQuestionModels,
                Total = testQuestions.Count()
            };

            ViewBag.TestCategoryId = testCategoryId;

            return Json(result);
        }

        #endregion

        #region GET :/TestQuestion/Create

        [HttpGet]
        public ActionResult TestQuestionCreate(int testCategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            TestCategory testCategory = _testService.GetTestCategoryById(testCategoryId);
            if (testCategory == null)
                return RedirectToAction("TestCategoryIndex");

            var model = new TestQuestionModel()
            {
                TestCategoryId = testCategoryId,
                IsSingleChoice = true,
                TestCategoryName = testCategory.TestCategoryName
            };

            model.IsActive = true;
            model.UpdatedOnUtc = System.DateTime.UtcNow;
            model.CreatedOnUtc = System.DateTime.UtcNow;

            return View(model);
        }

        #endregion

        #region POST:/TestQuestion/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult TestQuestionCreate(TestQuestionModel model, IEnumerable<HttpPostedFileBase> attachments, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                TestQuestion testQuestion = model.ToEntity();

                if (attachments != null)
                {
                    HttpPostedFileBase httpPostedFile = Request.Files[0];
                    if (httpPostedFile != null)
                    {
                        Stream stream = httpPostedFile.InputStream;
                        string fileName = Path.GetFileName(httpPostedFile.FileName);
                        //string contentType = httpPostedFile.ContentType;
                        //string fileExtension = Path.GetExtension(fileName);
                        //if (!String.IsNullOrEmpty(fileExtension))
                        //    fileExtension = fileExtension.ToLowerInvariant();


                        var fileBinary = new byte[stream.Length];
                        stream.Read(fileBinary, 0, fileBinary.Length);

                        // save image to server
                        testQuestion.ImageFileLocation = _testService.UploadTestImage(testQuestion.Id, fileBinary, fileName);
                    }
                }

                testQuestion.CreatedOnUtc = System.DateTime.UtcNow;
                testQuestion.UpdatedOnUtc = System.DateTime.UtcNow;


                // add new question
                _testService.InsertTestQuestion(testQuestion);


                //activity log
                _activityLogService.InsertActivityLog("AddNewTestQuestion", _localizationService.GetResource("ActivityLog.AddNewTestQuestion"), testQuestion.Question);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.TestQuestion.Added"));
                return continueEditing ? RedirectToAction("TestQuestionEdit", new { TestQuestionId = testQuestion.Id }) : RedirectToAction("TestQuestionIndex", new { TestCategoryId = testQuestion.TestCategoryId });

            }

            return View(model);
        }

        #endregion 

        #region GET :/TestQuestion/Edit

        [HttpGet]
        public ActionResult TestQuestionEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            TestQuestion testQuestion = _testService.GetTestQuestionById(id);
            if (testQuestion == null)
                return RedirectToAction("TestCategoryIndex");

            TestQuestionModel model = testQuestion.ToModel();
            model.ImageFileUrl = _testService.GetTestImageUrl(testQuestion.ImageFileLocation);

            TestCategory testCategory = _testService.GetTestCategoryById(testQuestion.TestCategoryId);
            model.TestCategoryName = testCategory.TestCategoryName;

            return View(model);
        }

        #endregion

        #region POST:/TestQuestion/Edit

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult TestQuestionEdit(TestQuestionModel model, IEnumerable<HttpPostedFileBase> attachments, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                TestQuestion testQuestion = _testService.GetTestQuestionById(model.Id);
                if (testQuestion == null)
                    return RedirectToAction("TestCategoryIndex");

                // To track for removing picture
                string oldImageFile = testQuestion.ImageFileLocation;
                

                testQuestion = model.ToEntity(testQuestion);


                if (attachments != null)
                {
                    HttpPostedFileBase httpPostedFile = Request.Files[0];
                    if (httpPostedFile != null)
                    {
                        // Delete existing image file on the server if exists
                        if (!string.IsNullOrWhiteSpace(testQuestion.ImageFileLocation))
                        {
                            string rootDirectory = _webHelper.GetRootDirectory();
                            string existingImageFile = Path.Combine(rootDirectory, testQuestion.ImageFileLocation);
                            // delete existing file
                            if (System.IO.File.Exists(existingImageFile))
                            {
                                System.IO.File.Delete(existingImageFile);
                            }
                        }


                        // get new image file
                        Stream stream = httpPostedFile.InputStream;
                        string fileName = Path.GetFileName(httpPostedFile.FileName);
                        //string contentType = httpPostedFile.ContentType;
                        //string fileExtension = Path.GetExtension(fileName);
                        //if (!String.IsNullOrEmpty(fileExtension))
                        //    fileExtension = fileExtension.ToLowerInvariant();

                        var fileBinary = new byte[stream.Length];
                        stream.Read(fileBinary, 0, fileBinary.Length);


                        // save image to server
                        testQuestion.ImageFileLocation = _testService.UploadTestImage(testQuestion.Id, fileBinary, fileName);
                    }
                }
                else
                {
                    // Remove existing picture only
                    if (!string.Equals(oldImageFile, model.ImageFileLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        string rootDirectory = _webHelper.GetRootDirectory();
                        string existingImageFile = Path.Combine(rootDirectory, oldImageFile);
                        // delete existing file
                        if (System.IO.File.Exists(existingImageFile))
                        {
                            System.IO.File.Delete(existingImageFile);
                        }
                        testQuestion.ImageFileLocation = null;
                    }
                }
                
                testQuestion.UpdatedOnUtc = System.DateTime.UtcNow;


                // Update question
                _testService.UpdateTestQuestion(testQuestion);


                //activity log
                _activityLogService.InsertActivityLog("UpdateTestQuestion", _localizationService.GetResource("ActivityLog.UpdateTestQuestion"), testQuestion.Question);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.TestQuestion.Updated"));
                return continueEditing ? RedirectToAction("TestQuestionEdit", new { id = testQuestion.Id }) : RedirectToAction("TestQuestionIndex", new { testCategoryId = testQuestion.TestCategoryId });
            }

            return View(model);
        }

        #endregion


        // TestChoice
        // ==============================

        #region GET :/TestChoiceIndex

        public ActionResult TestChoiceIndex(int testQuestionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            TestQuestion testQuestion = _testService.GetTestQuestionById(testQuestionId);
            var testChoices = _testService.GetTestChoicesByTestQuestionId(testQuestionId);

            List<TestChoiceModel> testChoicesModels = new List<TestChoiceModel>();
            foreach (var item in testChoices)
            {
                TestChoiceModel model = item.ToModel();
                model.ImageFileUrl = _testService.GetTestImageUrl(item.ImageFileLocation);
                model.TestQuestionQuestion = testQuestion.Question;
                testChoicesModels.Add(model);
            }

            // for navigation
            ViewBag.TestCategoryId = testQuestion.TestCategoryId;
            ViewBag.TestQuestionId = testQuestionId;

            return View(testChoicesModels);
        }

        #endregion

        #region GET :/TestChoiceIndex/Create

        public ActionResult TestChoiceCreate(int testQuestionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            TestQuestion testQuestion = _testService.GetTestQuestionById(testQuestionId);

            TestChoiceModel testChoiceModel = new TestChoiceModel()
            {
                TestQuestionId = testQuestionId,
                TestQuestionQuestion = testQuestion.Question
            };

            testChoiceModel.IsActive = true;
            testChoiceModel.UpdatedOnUtc = System.DateTime.UtcNow;
            testChoiceModel.CreatedOnUtc = System.DateTime.UtcNow;

            return View(testChoiceModel);
        }

        #endregion

        #region POST:/TestChoiceIndex/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult TestChoiceCreate(TestChoiceModel model, IEnumerable<HttpPostedFileBase> attachments, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                TestChoice testChoice = model.ToEntity();


                if (attachments != null)
                {
                    HttpPostedFileBase httpPostedFile = Request.Files[0];
                    if (httpPostedFile != null)
                    {
                        Stream stream = httpPostedFile.InputStream;
                        string fileName = Path.GetFileName(httpPostedFile.FileName);
                        //string contentType = httpPostedFile.ContentType;
                        //string fileExtension = Path.GetExtension(fileName);
                        //if (!String.IsNullOrEmpty(fileExtension))
                        //    fileExtension = fileExtension.ToLowerInvariant();


                        var fileBinary = new byte[stream.Length];
                        stream.Read(fileBinary, 0, fileBinary.Length);

                        // save image to server
                        testChoice.ImageFileLocation = _testService.UploadTestImage(testChoice.TestQuestionId, fileBinary, fileName);
                    }
                }

                testChoice.CreatedOnUtc = System.DateTime.UtcNow;
                testChoice.UpdatedOnUtc = System.DateTime.UtcNow;


                // add new choice
                _testService.InsertTestChoice(testChoice);


                //activity log
                _activityLogService.InsertActivityLog("AddNewTestChoice", _localizationService.GetResource("ActivityLog.AddNewTestChoice"), testChoice.TestChoiceText);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.TestChoice.Added"));
                return continueEditing ? RedirectToAction("TestChoiceEdit", new { id = testChoice.Id }) : RedirectToAction("TestChoiceIndex", new { TestQuestionId = testChoice.TestQuestionId });
            }

            return View(model);
        }

        #endregion 

        #region GET :/TestChoiceEdit/Edit

        [HttpGet]
        public ActionResult TestChoiceEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            TestChoice testChoice = _testService.GetTestChoiceById(id);
            if (testChoice == null)
                return RedirectToAction("TestCategoryIndex");

            TestChoiceModel model = testChoice.ToModel();
            model.ImageFileUrl = _testService.GetTestImageUrl(testChoice.ImageFileLocation);

            TestQuestion testQuestion = _testService.GetTestQuestionById(testChoice.TestQuestionId);
            model.TestQuestionQuestion = testQuestion.Question;

            return View(model);       
        }
        #endregion

        #region POST:/TestChoiceEdit/Edit

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult TestChoiceEdit(TestChoiceModel model, IEnumerable<HttpPostedFileBase> attachments, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                TestChoice testChoice = _testService.GetTestChoiceById(model.Id);
                if (testChoice == null)
                    return RedirectToAction("TestCategoryIndex");

                // To track for removing picture
                string oldImageFile = testChoice.ImageFileLocation;


                testChoice = model.ToEntity(testChoice);


                if (attachments != null)
                {
                    HttpPostedFileBase httpPostedFile = Request.Files[0];
                    if (httpPostedFile != null)
                    {
                        // Delete existing image file on the server if exists
                        if (!string.IsNullOrWhiteSpace(testChoice.ImageFileLocation))
                        {
                            string rootDirectory = _webHelper.GetRootDirectory();
                            string existingImageFile = Path.Combine(rootDirectory, testChoice.ImageFileLocation);
                            // delete the file
                            if (System.IO.File.Exists(existingImageFile))
                            {
                                System.IO.File.Delete(existingImageFile);
                            }
                        }


                        // get new image file
                        Stream stream = httpPostedFile.InputStream;
                        string fileName = Path.GetFileName(httpPostedFile.FileName);
                        //string contentType = httpPostedFile.ContentType;
                        //string fileExtension = Path.GetExtension(fileName);
                        //if (!String.IsNullOrEmpty(fileExtension))
                        //    fileExtension = fileExtension.ToLowerInvariant();

                        var fileBinary = new byte[stream.Length];
                        stream.Read(fileBinary, 0, fileBinary.Length);


                        // save image to server
                        testChoice.ImageFileLocation = _testService.UploadTestImage(testChoice.TestQuestionId, fileBinary, fileName);
                    }
                }
                else
                {
                    // Remove existing picture only
                    if (!string.Equals(oldImageFile, model.ImageFileLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        string rootDirectory = _webHelper.GetRootDirectory();
                        string existingImageFile = Path.Combine(rootDirectory, oldImageFile);
                        // delete existing file
                        if (System.IO.File.Exists(existingImageFile))
                        {
                            System.IO.File.Delete(existingImageFile);
                        }
                        testChoice.ImageFileLocation = null;
                    }
                }


                testChoice.UpdatedOnUtc = System.DateTime.UtcNow;


                // update choice
                _testService.UpdateTestChoice(testChoice);


                //activity log
                _activityLogService.InsertActivityLog("UpdateTestChoice", _localizationService.GetResource("ActivityLog.UpdateTestChoice"), testChoice.TestChoiceText);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.TestChoice.Updated"));
                return continueEditing ? RedirectToAction("TestChoiceEdit", new { id = testChoice.Id }) : RedirectToAction("TestChoiceIndex", new { testQuestionId = testChoice.TestQuestionId });
            }

            return View(model);
        }

        #endregion 


        #region TestMaterials
        public ActionResult TestMaterialsIndex()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult TestMaterialsList(DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();
            var materials = _testService.GetAllTestMaterials();
            return Json(materials.ToDataSourceResult(request,m=>m.ToModel()));
        }

        public ActionResult _UploadTestMaterial()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult DeleteMaterial(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();
            var entity = _testService.Retrieve(id);
            _testService.Delete(entity);
            return View();
        }

        [HttpPost]
        public ActionResult SaveMaterials(IEnumerable<HttpPostedFileBase> attachments, int? categoryId )
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();
            foreach (var file in attachments)
            {
                // prepare
                var fileName = Path.GetFileName(file.FileName);
                var contentType = file.ContentType;

                // not supported file format
                AttachmentType attachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(fileName));
                if (attachmentType == null)
                    return Content("File format is not supported, please contact administrator.");

                using (Stream stream = file.InputStream)
                {
                    var fileBinary = new byte[stream.Length];
                    stream.Read(fileBinary, 0, fileBinary.Length);

                    // upload attachment
                    string result = _testService.UploadTestMaterial(fileBinary, fileName, attachmentType.Id,categoryId,contentType);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return Content(result);
                    }
                }
            }


            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DownloadTestMaterial(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTests))
                return AccessDeniedView();

            var material = _testService.RetrieveByGuid(guid);
            if (material == null)
                return RedirectToAction("List");


            return File(material.AttachmentFile, material.ContentType, material.AttachmentFileName);

        }
        #endregion

    }
}
