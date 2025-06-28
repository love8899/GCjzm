using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.Test
{
    public partial class TestService : ITestService
    {

        #region Constants - TestCategory

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : testcategory ID
        /// </remarks>
        private const string TESTCATEGORIES_BY_ID_KEY = "Wfm.testcategory.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string TESTCATEGORIES_ALL_KEY = "Wfm.testcategory.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string TESTCATEGORIES_PATTERN_KEY = "Wfm.testcategory.";

        #endregion

        #region Constants - TestQuestion

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : testquestion ID
        /// </remarks>
        private const string TESTQUESTIONS_BY_ID_KEY = "Wfm.testquestion.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string TESTQUESTIONS_ALL_KEY = "Wfm.testquestion.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string TESTQUESTIONS_PATTERN_KEY = "Wfm.testquestion.";

        #endregion

        #region Constants - TestChoice

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : testchoice ID
        /// </remarks>
        private const string TESTCHOICES_BY_ID_KEY = "Wfm.testchoice.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string TESTCHOICES_ALL_KEY = "Wfm.testchoice.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string TESTCHOICES_PATTERN_KEY = "Wfm.testchoice.";

        #endregion


        #region Fields

        private readonly IRepository<TestCategory> _testCategoryRepository;
        private readonly IRepository<TestQuestion> _testQuestionRepository;
        private readonly IRepository<CandidateTestResult> _candidateTestResultRepository;
        private readonly IRepository<TestChoice> _testChoiceRepository;
        private readonly IRepository<TestMaterial> _testMaterialRepository;
        private readonly MediaSettings _mediaSettings;
        private readonly IWebHelper _webHelper;

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public TestService(
            IRepository<TestCategory> testCategoryRepository,
            IRepository<TestQuestion> testQuestionRepository,
            IRepository<TestChoice> testChoiceRepository,
            IRepository<CandidateTestResult> candidateTestResultRepository,
            MediaSettings mediaSettings,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<TestMaterial> testMaterialRepository
            )
        {
            _testCategoryRepository = testCategoryRepository;
            _testQuestionRepository = testQuestionRepository;
            _testChoiceRepository = testChoiceRepository;
            _candidateTestResultRepository = candidateTestResultRepository;
            _mediaSettings = mediaSettings;
            _webHelper = webHelper;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _testMaterialRepository = testMaterialRepository;
        }

        #endregion

        #region Utilities

        protected string GetTestImageFileSubDirectory()
        {
            string year = System.DateTime.Now.ToString("yyyy");
            string month = System.DateTime.Now.ToString("MM");
            string day = System.DateTime.Now.ToString("dd");

            return Path.Combine(_mediaSettings.TestImageLocation, year, month);
        }

        protected virtual void DeleteTestQuestionImageFileOnFileSystem(TestQuestion testQuestion)
        {
            if (testQuestion == null)
                throw new ArgumentNullException("testQuestion");

            // web root directory
            string rootDirectory = _webHelper.GetRootDirectory();
            string imageFileWithPath = Path.Combine(rootDirectory, testQuestion.ImageFileLocation);

            // delete the file
            if (File.Exists(imageFileWithPath))
            {
                File.Delete(imageFileWithPath);
            }
        }


        #endregion

        #region Common

        public virtual string UploadTestImage(int testQuestionId, byte[] imageBinary, string fileName)
        {
            if (testQuestionId == 0)
                return string.Empty;
            if (imageBinary == null || imageBinary.Length == 0)
                return string.Empty;


            // web root directory
            string rootDirectory = _webHelper.GetRootDirectory();


            // get image root directory
            string imageSubDirectory = _mediaSettings.TestImageLocation;
            string imageRootDirectory = Path.Combine(rootDirectory, imageSubDirectory, testQuestionId.ToString());
            if (!Directory.Exists(imageRootDirectory))
            {
                Directory.CreateDirectory(imageRootDirectory);
            }


            // get stored file name
            string fileExtension = Path.GetExtension(fileName);
            string destinationFileName = Guid.NewGuid().ToString() + fileExtension;
            string destinationFileNameWithPath = Path.Combine(imageRootDirectory, destinationFileName);
          

            // save in file
            File.WriteAllBytes(destinationFileNameWithPath, imageBinary);


            // return relative file path and file name
            return Path.Combine(imageSubDirectory, testQuestionId.ToString(), destinationFileName);
        }

        public virtual string GetTestImageUrl(string imageFileLocation, string franchiseLocation = null)
        {
            if (string.IsNullOrWhiteSpace(imageFileLocation))
                return string.Empty;

            franchiseLocation = !String.IsNullOrEmpty(franchiseLocation)
                                    ? franchiseLocation
                                    : _webHelper.GetFranchiseUrl();

            var url = franchiseLocation.TrimEnd('/') + "/";

            url = url + imageFileLocation.Replace(@"\", "/").TrimStart('/');

            return url;
        }

        #endregion

        #region TestCategory

        public void InsertTestCategory(TestCategory testCategory)
        {
            if (testCategory == null)
                throw new ArgumentNullException("testCategory");

            //insert
            _testCategoryRepository.Insert(testCategory);

            //cache
            _cacheManager.RemoveByPattern(TESTCATEGORIES_PATTERN_KEY);
        }

        public void UpdateTestCategory(TestCategory testCategory)
        {
            if (testCategory == null)
                throw new ArgumentNullException("testCategory");

            _testCategoryRepository.Update(testCategory);

            //cache
            _cacheManager.RemoveByPattern(TESTCATEGORIES_PATTERN_KEY);
        }

        public void DeleteTestCategory(TestCategory testCategory)
        {
            if (testCategory == null)
                throw new ArgumentNullException("testCategory");

            _testCategoryRepository.Delete(testCategory);

            //cache
            _cacheManager.RemoveByPattern(TESTCATEGORIES_PATTERN_KEY);
        }

        public TestCategory GetTestCategoryById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _testCategoryRepository.GetById(id);
            return _testCategoryRepository.Table
                .Where(x => x.Id == id)
                .Select(x => new {
                        x,
                        TestQuestions = x.TestQuestions.Where(y => y.IsActive)
                    })
                .AsEnumerable()
                .Select(x => x.x)
                .FirstOrDefault();

            // Using caching
            //string key = string.Format(TESTCATEGORIES_BY_ID_KEY, id);
            //return _cacheManager.Get(key, () => _testCategoryRepository.GetById(id));
        }

        public IList<TestCategory> GetAllCategories(bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------------
            //var query = _testCategoryRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(c => c.IsDeleted == false);

            //query = from b in query
            //        orderby b.DisplayOrder, b.TestCategoryName
            //        select b;

            //return query.ToList();



            //using cache
            //-----------------------------
            string key = string.Format(TESTCATEGORIES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _testCategoryRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                query = from b in query
                        orderby b.DisplayOrder, b.TestCategoryName
                        select b;

                return query.ToList();
            });
        }

        public IList<TestCategory> GetAllRequiredTestCategories(bool showInactive = false, bool showHidden = false)
        {
            string key = string.Format(TESTCATEGORIES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _testCategoryRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                query = query.Where(x => x.IsRequiredWhenRegistration);
                query = from b in query
                        orderby b.DisplayOrder, b.TestCategoryName
                        select b;

                return query.ToList();
            });
        }
        #endregion

        #region TestQuestions

        public void InsertTestQuestion(TestQuestion testQuestion)
        {
            if (testQuestion == null)
                throw new ArgumentNullException("testQuestion");

            _testQuestionRepository.Insert(testQuestion);

            //cache
            _cacheManager.RemoveByPattern(TESTQUESTIONS_PATTERN_KEY);
        }

        public void UpdateTestQuestion(TestQuestion testQuestion)
        {
            if (testQuestion == null)
                throw new ArgumentNullException("testQuestion");

            _testQuestionRepository.Update(testQuestion);

            //cache
            _cacheManager.RemoveByPattern(TESTQUESTIONS_PATTERN_KEY);
        }

        public void DeleteTestQuestion(TestQuestion testQuestion)
        {
            if (testQuestion == null)
                throw new ArgumentNullException("testQuestion");

            //delete from file system
            DeleteTestQuestionImageFileOnFileSystem(testQuestion);

            //delete from database
            _testQuestionRepository.Delete(testQuestion);

            //cache
            _cacheManager.RemoveByPattern(TESTQUESTIONS_PATTERN_KEY);
        }

        public TestQuestion GetTestQuestionById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _testQuestionRepository.GetById(id);

            // Using caching
            string key = string.Format(TESTQUESTIONS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _testQuestionRepository.GetById(id));
        }

        public List<TestQuestion> GetTestQuestionsByTestCategoryId(int testCategoryId)
        {
            //no cache
            //-----------------------------
            //var query = _testQuestionRepository.Table;

            //query = from t in query
            //    where t.TestCategoryId == testCategoryId
            //    orderby t.DisplayOrder
            //    select t;

            //return query.ToList();



            //using cache
            //-----------------------------
            string key = string.Format(TESTQUESTIONS_ALL_KEY, testCategoryId);
            return _cacheManager.Get(key, () =>
            {
                var query = _testQuestionRepository.Table;

                query = from t in query
                        where t.TestCategoryId == testCategoryId
                        orderby t.DisplayOrder
                        select t;

                return query.ToList();
            });
        }

        #endregion

        #region TestChoices
        public void InsertTestChoice(TestChoice testChoice)
        {
            if (testChoice == null)
                throw new ArgumentNullException("testChoice");

            _testChoiceRepository.Insert(testChoice);

            //cache
            _cacheManager.RemoveByPattern(TESTCHOICES_PATTERN_KEY);
        }

        public void UpdateTestChoice(TestChoice testChoice)
        {
            if (testChoice == null)
                throw new ArgumentNullException("testChoice");

            _testChoiceRepository.Update(testChoice);

            //cache
            _cacheManager.RemoveByPattern(TESTCHOICES_PATTERN_KEY);
        }

        public void DeleteTestChoice(TestChoice testChoice)
        {
            if (testChoice == null)
                throw new ArgumentNullException("testChoice");

            _testChoiceRepository.Delete(testChoice);

            //cache
            _cacheManager.RemoveByPattern(TESTCHOICES_PATTERN_KEY);
        }

        public TestChoice GetTestChoiceById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _testChoiceRepository.GetById(id);

            // Using caching
            string key = string.Format(TESTCHOICES_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _testChoiceRepository.GetById(id));
        }

        public IList<TestChoice> GetTestChoicesByTestQuestionId(int testQuestionId)
        {
            //no cache
            //-----------------------------
            //var query = _testChoiceRepository.Table;

            //query = from t in query
            //        where t.TestQuestionId == testQuestionId
            //        select t;

            //return query.ToList();



            //using cache
            //-----------------------------
            string key = string.Format(TESTCHOICES_ALL_KEY, testQuestionId);
            return _cacheManager.Get(key, () =>
            {
                var query = _testChoiceRepository.Table;

                query = from t in query
                        where t.TestQuestionId == testQuestionId
                        select t;

                return query.ToList();
            });
        }

        #endregion


        #region Test Material
        public void Create(TestMaterial entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            entity.CreatedOnUtc = entity.UpdatedOnUtc = DateTime.UtcNow;
            _testMaterialRepository.Insert(entity);
        }

        public TestMaterial Retrieve(int id)
        {
            if (id == 0)
                return null;
            return _testMaterialRepository.GetById(id);
            
        }

        public TestMaterial RetrieveByGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return null;
            return _testMaterialRepository.Table.Where(x => x.TestMaterialGuid == guid).FirstOrDefault();
        }
        public void Update(TestMaterial entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _testMaterialRepository.Update(entity);
        }

        public void Delete(TestMaterial entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            entity.UpdatedOnUtc = DateTime.UtcNow;
            entity.IsActive = false;
            entity.IsDeleted = true;
            _testMaterialRepository.Update(entity);
        }

        public IQueryable<TestMaterial> GetAllTestMaterials(bool? onlyVideo=null,bool showInactive=false)
        {
            var result = _testMaterialRepository.Table.Where(x=>!x.IsDeleted);
            if (!showInactive)
                result = result.Where(x => x.IsActive);
            if (onlyVideo.HasValue)
            {
                if (onlyVideo.Value)
                    result = result.Where(x => x.AttachmentType.TypeName == "Video");
                else
                    result = result.Where(x => x.AttachmentType.TypeName != "Video");
            }
            return result;
        }

        public string UploadTestMaterial(byte[] attachment, string fileName, int attachmentTypeId,int? testCategoryId,string contentType)
        {
            string result = string.Empty;

            if (attachment == null || attachment.Length == 0)
                return result;
            if (string.IsNullOrWhiteSpace(fileName))
                return result;


            // get stored file name
            string fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            string sourceFileName = Path.GetFileName(fileName);
            // add new attachment
            var newAttachment = new TestMaterial()
            {
                AttachmentTypeId = attachmentTypeId,
                AttachmentFileName = sourceFileName,
                ContentType = contentType,
                IsActive = true,
                IsDeleted = false,
                AttachmentFile = attachment,
                TestCategoryId = testCategoryId
            };


            Create(newAttachment);

            // save in file
            //File.WriteAllBytes(destinationFileNameWithPath, attachmentBinary);
            return result;
        }
        #endregion

    }
}
