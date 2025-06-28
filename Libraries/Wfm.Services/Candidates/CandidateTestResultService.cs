using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Media;

namespace Wfm.Services.Candidates
{
    public partial class CandidateTestResultService : ICandidateTestResultService
    {

        #region Fields

        private readonly IRepository<CandidateTestResult> _candidateTestResultRepository;
        private readonly MediaSettings _mediaSettings;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CandidateTestResultService(
            IRepository<CandidateTestResult> candidateTestResultRepository,
            MediaSettings mediaSettings,
            IWebHelper webHelper
            )
        {
            _candidateTestResultRepository = candidateTestResultRepository;
            _mediaSettings = mediaSettings;
            _webHelper = webHelper;
        }

        #endregion


        #region Utilities

        protected virtual string GetTestResultSubDirectory()
        {
            string year = System.DateTime.Now.ToString("yyyy");
            string month = System.DateTime.Now.ToString("MM");
            string day = System.DateTime.Now.ToString("dd");

            return Path.Combine(_mediaSettings.CandidateTestResultFileLocation, year, month, day);
        }

        #endregion

        #region CRUD

        public void InsertCandidateTestResult(CandidateTestResult candidateTestResult)
        {
            if (candidateTestResult == null)
                throw new ArgumentNullException("candidateTestResult");

            _candidateTestResultRepository.Insert(candidateTestResult);
        }

        public void UpdateCandidateTestResult(CandidateTestResult candidateTestResult)
        {
            if (candidateTestResult == null)
                throw new ArgumentNullException("candidateTestResult");

            _candidateTestResultRepository.Update(candidateTestResult);
        }

        public void DeleteCandidateTestResult(CandidateTestResult candidateTestResult)
        {
            if (candidateTestResult == null)
                throw new ArgumentNullException("candidateTestResult");

            _candidateTestResultRepository.Delete(candidateTestResult);
        }



        public string SaveCandiateTestResultToFile(int candidateId, int testCategoryId, StringBuilder sbTestResultContent)
        {
            // web root directory
            string rootDirectory = _webHelper.GetRootDirectory();

            // Prepare directory
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");
            string day = DateTime.Now.ToString("dd");
            string hour = DateTime.Now.ToString("HH");
            string minute = DateTime.Now.ToString("mm");
            string dateStr = year + month + day + hour + minute;

            string testResultFileName = candidateId.ToString() + "_" + testCategoryId.ToString() + "_" + dateStr + "_" + Guid.NewGuid().ToString() + ".txt";

            string testFileHomeDirectory = Path.Combine(rootDirectory, _mediaSettings.CandidateTestResultFileLocation, year, month, day);
            if (!System.IO.Directory.Exists(testFileHomeDirectory))
            {
                System.IO.Directory.CreateDirectory(testFileHomeDirectory);
            }

            // Save result file
            string testFileWithPath = testFileHomeDirectory + "\\" + testResultFileName;
            using (var fileStream = new FileStream(testFileWithPath, FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(sbTestResultContent.ToString());
                   // streamWriter.Close();
                }
                fileStream.Close();
            }

            // Construct data store path for record
            string relativeStoredDirectory = Path.Combine(_mediaSettings.CandidateTestResultFileLocation, year, month, day); // no root path for database store
            string relativeStoredFileWithPath = relativeStoredDirectory + "\\" + testResultFileName;

            return relativeStoredFileWithPath;
        }

        #endregion


        public CandidateTestResult GetCandidateTestResultById(int id)
        {
            if (id == 0)
                return null;

            return _candidateTestResultRepository.GetById(id);
        }

        public List<CandidateTestResult> GetCandidateTestResultsByCandidateId(int candidateId)
        {
            var query = _candidateTestResultRepository.Table;

            query = from c in query
                    where c.CandidateId == candidateId
                    orderby c.CreatedOnUtc descending
                    select c;

            return query.ToList();
        }

    }
}
