using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Features;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;


namespace Wfm.Services.Features
{
    public partial class UserFeatureService : IUserFeatureService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<UserFeature> _userFeatureRepository;
        private readonly ICompanyService _companyService;
        private readonly IFranchiseService _franchiseService;
        private readonly IFeatureService _featureService;
        private readonly FeatureSettings _featureSettings;

        #endregion 


        #region Ctor

        public UserFeatureService(
            IWorkContext workContext,
            IRepository<UserFeature> userFeatureRepository, 
            ICompanyService companyService,
            IFranchiseService franchiseService,
            IFeatureService featureService,
            FeatureSettings featureSettings)
        {
            _workContext = workContext;
            _userFeatureRepository = userFeatureRepository;
            _companyService = companyService;
            _franchiseService = franchiseService;
            _featureService = featureService;
            _featureSettings = featureSettings;
        }

        #endregion


        #region CRUD

        public void InsertUserFeature(UserFeature userFeature)
        {
            if (userFeature == null)
                throw new ArgumentNullException("userFeature");

            _userFeatureRepository.Insert(userFeature);
        }

        public void UpdateUserFeature(UserFeature userFeature)
        {
            if (userFeature == null)
                throw new ArgumentException("userFeature");

            _userFeatureRepository.Update(userFeature);
        }

        public void DeleteUserFeature(UserFeature userFeature)
        {
            if (userFeature == null)
                throw new ArgumentException("userFeature");

            _userFeatureRepository.Delete(userFeature);
        }

        #endregion


        #region UserFeature

        public UserFeature GetUserFeatureById(int id)
        {
            if (id == 0)
                return null;

            var query = _userFeatureRepository.Table;

            query = query.Where(x => x.Id == id);

            return query.FirstOrDefault();
        }

        public void DeleteAllUserFeaturesByCompanyGuid(Guid? guid)
        {
            var company = _companyService.GetCompanyByGuid(guid);
            if (company == null)
                return;
            var features = _userFeatureRepository.Table.Where(x => x.UserId == company.Id);
            if (features.Count() > 0)
                _userFeatureRepository.Delete(features);

        }
        #endregion


        #region LIST

        public IQueryable<UserFeature> GetAllUserFeatures()
        {
            var query = _userFeatureRepository.Table;

            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            return query;
        }


        public IList<UserFeature> GetAllUserFeaturesByUserId(int userId, bool activeOnly = true)
        {
            var query = GetAllUserFeatures().Where(x => x.UserId == userId);

            if (activeOnly)
                query = query.Where(x => x.IsActive && x.Feature.IsActive);

            return query.ToList();
        }


        public List<string> GetAllUserFeatureNamesByUserId(int userId, string area="Client",bool activeOnly = true)
        {
            var query = GetAllUserFeatures().Where(x => x.UserId == userId&&x.Area==area);

            if (activeOnly)
                query = query.Where(x => x.IsActive && x.Feature.IsActive);

            return query.Select(x => x.Feature.Name).ToList();
        }

        #endregion


        #region Enable new features

        public void EnableNewFeatureForAllUsers(Feature feature)
        {
            if (feature == null)
                return;

            var Ids = new List<int>();
            if (feature.Area == "Client")
                Ids = _companyService.GetAllCompaniesAsQueryable(_workContext.CurrentAccount, showInactive: true).Select(x => x.Id).ToList();
            else if (feature.Area == "Admin")
                Ids = _franchiseService.GetAllVendorsAsQueryable(_workContext.CurrentAccount, showInactive: true).Select(x => x.Id).ToList();

            foreach (var Id in Ids)
            {
                this.InsertUserFeature(new UserFeature
                {
                    Area = feature.Area,
                    UserId = Id,
                    FeatureId = feature.Id,
                    IsActive = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                });
            }
        }


        public void EnableAllFeaturesForNewUser(string area, int userId)
        {
            var areas = new List<string>() {"Admin", "Client"};
            if (!areas.Contains(area) || userId <= 0)
                return;

            var features = _featureService.GetAllFeatures(activeOnly: false);
            var featureIds = features.Select(x => x.Id).ToList();
            var excludedIds = Enumerable.Empty<int>();
            var excludedCodes = area == "Client" ? _featureSettings.ClientFeaturesNotEnabledByDefault : string.Empty;
            if (!String.IsNullOrWhiteSpace(excludedCodes))
            {
                var codeList = excludedCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                excludedIds = features.Where(x => x.Area == area && codeList.Contains(x.Code)).Select(x => x.Id).AsEnumerable();
            }
            foreach (var Id in featureIds)
            {
                this.InsertUserFeature(new UserFeature
                {
                    Area = area,
                    UserId = userId,
                    FeatureId = Id,
                    IsActive = !excludedIds.Contains(Id),
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                });
            }
        }

        #endregion

        #region User has the feature or not
        public bool CheckFeatureByName(int userId, string name)
        {
            List<string> features = GetAllUserFeatureNamesByUserId(userId);
            if (features.Any(x => x == name))
                return true;
            return false;
        }

        public bool CheckFeatureByCode(string featureCode)
        {
            var features = this.GetAllUserFeaturesByUserId(_workContext.CurrentAccount.Id);
            if (features.Any(x => x.Feature.Code == featureCode))
                return true;
            return false;
        }
        #endregion
    }
}
