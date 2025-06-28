using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using System.Web.Mvc;

namespace Wfm.Services.Companies
{
    public class ActivityTypeService:IActivityTypeService
    {
        #region Field
        private readonly IRepository<ActivityType> _activityTypeRepository;
        #endregion

        #region CTOR
        public ActivityTypeService(IRepository<ActivityType> activityTypeRepository)
        {
            _activityTypeRepository = activityTypeRepository;
        }
        #endregion

        #region CRUD
        public void Create(ActivityType entity)
        {
            if (entity == null)
                throw new ArgumentNullException("ActivityType");

            _activityTypeRepository.Insert(entity);
        }

        public ActivityType Retrieve(int id)
        {
            var activityType = _activityTypeRepository.GetById(id);
            return activityType;
        }

        public void Update(ActivityType entity)
        {
            if (entity == null)
                throw new ArgumentNullException("ActivityType");

            _activityTypeRepository.Update(entity);
        }

        public void Delete(ActivityType entity)
        {
            if (entity == null)
                throw new ArgumentNullException("ActivityType");

            _activityTypeRepository.Delete(entity);
        }
        #endregion

        #region Method
        public IList<SelectListItem> GetAllActivityTypesForDropDownList()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "None", Value = "0" });        
            var activityTypes = _activityTypeRepository.TableNoTracking.Select(x => new SelectListItem() { Text = x.TypeName, Value = x.Id.ToString() });
            types.AddRange(activityTypes);
            return types;
        }

        public IList<ActivityType> GetAllActivityTypes()
        {
            return _activityTypeRepository.Table.ToList();
        }
        #endregion
    }
}
