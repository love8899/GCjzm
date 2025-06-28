using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Features;


namespace Wfm.Services.Features
{
    public partial interface IFeatureService
    {
        #region CRUD

        void InsertFeature(Feature feature);

        void UpdateFeature(Feature feature);

        void DeleteFeature(Feature feature);

        #endregion


        #region Feature

        Feature GetFeatureById(int id);

        IList<Feature> GetFeaturesByAreaAndNameAndCode(string area, string code, string name);

        bool IsFeatureEnabled(string area, string name);

        #endregion


        #region LIST

        IQueryable<Feature> GetAllFeatures(bool activeOnly = true);

        #endregion

    }
}
