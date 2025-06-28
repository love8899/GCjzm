using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial class ResumeService : IResumeService
    {
        #region Fields

        private readonly IRepository<Resume> _resumes;

        #endregion


        #region Ctor

        public ResumeService(IRepository<Resume> resume)
        {
            _resumes = resume;
        }

        #endregion


        public void InsertResume(Resume resume)
        {
            if (resume == null)
                throw new ArgumentNullException("resume");

            resume.CreatedOnUtc = resume.UpdatedOnUtc = DateTime.UtcNow;

            _resumes.Insert(resume);
        }


        public void UpdateResume(Resume resume)
        {
            if (resume == null)
                throw new ArgumentNullException("resume");

            resume.UpdatedOnUtc = DateTime.UtcNow;

            _resumes.Update(resume);
        }


        public void InsertOrSkip(Resume resume)
        {
            if (resume == null)
                throw new ArgumentNullException("resume");

            var existing = GetResumeByAccountAndUniqueId(resume.Account, resume.UniqueId);
            if (existing == null)
                InsertResume(resume);
        }


        public Resume GetResumeById(int resumeId)
        {
            if (resumeId == 0)
                return null;

            return _resumes.GetById(resumeId);

        }


        public IQueryable<Resume> GetAllResumesAsQueryable()
        {
            return _resumes.Table;
        }


        public Resume GetResumeByAccountAndUniqueId(string account, int id)
        {
            return GetAllResumesAsQueryable().FirstOrDefault(x => x.Account == account && x.UniqueId == id);
        }


        public int GetLastUniqueId(string account)
        {
            var last = GetAllResumesAsQueryable().Where(x => x.Account == account)
                .OrderByDescending(x => x.UniqueId).FirstOrDefault();

            return last != null ? last.UniqueId : 0;
        }


        public IQueryable<Resume> SearchByAllWords(IQueryable<Resume> resumes, string[] words,
            bool inSubject = true, bool inBody = true, bool inAttachment = true)
        {
            var result = from r in resumes
                         where words.All(w => (inSubject && r.Subject.Contains(w)) ||
                                              (inBody && r.Body.Contains(w)) ||
                                              (inAttachment && r.AttachmentText.Contains(w)))
                         select r;

            return result.Distinct();
        }


        public IQueryable<Resume> SearchByAnyOfWords(IQueryable<Resume> resumes, string[] words,
            bool inSubject = true, bool inBody = true, bool inAttachment = true)
        {
            var result = from r in resumes
                         from w in words
                         where (inSubject && r.Subject.Contains(w)) ||
                               (inBody && r.Body.Contains(w)) ||
                               (inAttachment && r.AttachmentText.Contains(w))
                         select r;

            return result.Distinct();
        }


        public IQueryable<Resume> GetAllResumesByCriteria(ResumeCriteria criteria)
        {
            var emptyResult = new List<Resume>().AsQueryable();
            var messages = GetAllResumesAsQueryable();

            if (criteria.StartDate != null)
            {
                var refDate = criteria.StartDate.ToUniversalTime();
                messages = messages.Where(x => x.Date >= refDate);
            }

            if (criteria.EndDate != null)
            {
                var refDate = criteria.EndDate.AddDays(1).ToUniversalTime();
                messages = messages.Where(x => x.Date < refDate);
            }

            if (!String.IsNullOrEmpty(criteria.SearchWords))
            {
                var words = criteria.SearchWords.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Any())
                {
                    messages = criteria.ByAllWords ?
                        SearchByAllWords(messages, words, criteria.InSubject, criteria.InBody, criteria.InAttachment) :
                        SearchByAnyOfWords(messages, words, criteria.InSubject, criteria.InBody, criteria.InAttachment);
                }
            }

            return messages;
        }

    }
}
