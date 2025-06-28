using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public class JobBoardService : IJobBoardService
    {
        #region Fields
        private readonly IRepository<JobBoard> _jobBoardRepository;
        #endregion

        #region Ctor
        public JobBoardService(IRepository<JobBoard> jobBoardRepository)
        {
            _jobBoardRepository = jobBoardRepository;
        }
        #endregion

        #region CRUD

        public void Create(JobBoard entity)
        {
            if (entity == null)
                throw new ArgumentNullException("JobBoard");
            _jobBoardRepository.Insert(entity);
        }

        public JobBoard Retrieve(int id)
        {
            if (id == 0)
                return null;

            return _jobBoardRepository.GetById(id);
        }

        public void Update(JobBoard entity)
        {
            if (entity == null)
                throw new ArgumentNullException("JobBoard");
            _jobBoardRepository.Update(entity);
        }

        public void Delete(JobBoard entity)
        {
            if (entity == null)
                throw new ArgumentNullException("JobBoard");
            entity.IsActive = false;
            entity.IsDeleted = true;
            _jobBoardRepository.Update(entity);
        }

        #endregion
        public IQueryable<JobBoard> GetAllJobBoardsAsQuerable(bool showInactive = false, bool showHidden = false)
        {
            var jobBoards = _jobBoardRepository.Table;
            if (!showInactive)
                jobBoards = jobBoards.Where(x => x.IsActive);
            if (!showHidden)
                jobBoards = jobBoards.Where(x => !x.IsDeleted);
            return jobBoards;
        }
        public IList<SelectListItem> GetAllJobBoards()
        {
            var jobBoards = GetAllJobBoardsAsQuerable().Select(x => new SelectListItem() { Text = x.JobBoardName, Value = x.Id.ToString() });
            return jobBoards.ToList();
        }
        public bool IsDuplicate(JobBoard entity)
        {
            var result = _jobBoardRepository.TableNoTracking.Where(x => x.JobBoardName == entity.JobBoardName&&x.IsActive&&!x.IsDeleted);
            if (entity.Id != 0)
                result = result.Where(x => x.Id != entity.Id);
            return result.Count() > 0;

        }

    }
}
