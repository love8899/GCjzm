using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Features;


namespace Wfm.Services.Features
{
    public partial interface IUserFeatureService
    {
        #region CRUD

        void InsertUserFeature(UserFeature userFeature);

        void UpdateUserFeature(UserFeature userFeature);

        void DeleteUserFeature(UserFeature userFeature);

        #endregion


        #region Feature

        UserFeature GetUserFeatureById(int id);

        void DeleteAllUserFeaturesByCompanyGuid(Guid? guid);
        #endregion


        #region LIST

        IQueryable<UserFeature> GetAllUserFeatures();

        IList<UserFeature> GetAllUserFeaturesByUserId(int userId, bool activeOnly = true);

        List<string> GetAllUserFeatureNamesByUserId(int userId, string area = "Client", bool activeOnly = true);

        #endregion


        #region Enable new features

        void EnableNewFeatureForAllUsers(Feature feature);

        void EnableAllFeaturesForNewUser(string area, int userId);

        #endregion
        #region User has the feature or not
        bool CheckFeatureByName(int userId, string name);
        bool CheckFeatureByCode(string featureCode);

        #endregion

    }
}
