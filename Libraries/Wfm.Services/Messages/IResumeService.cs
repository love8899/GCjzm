using System.Linq;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial interface IResumeService
    {
        void InsertResume(Resume resume);

        void UpdateResume(Resume resume);

        void InsertOrSkip(Resume resume);

        Resume GetResumeById(int resumeId);

        IQueryable<Resume> GetAllResumesAsQueryable();

        Resume GetResumeByAccountAndUniqueId(string account, int id);

        int GetLastUniqueId(string acount);

        IQueryable<Resume> SearchByAllWords(IQueryable<Resume> resumes, string[] words,
            bool inSubject = true, bool inBody = true, bool inAttachment = true);

        IQueryable<Resume> SearchByAnyOfWords(IQueryable<Resume> resumes, string[] words,
            bool inSubject = true, bool inBody = true, bool inAttachment = true);

        IQueryable<Resume> GetAllResumesByCriteria(ResumeCriteria criteria);
    }
}
