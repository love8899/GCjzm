using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Features;
using Wfm.Data;


namespace Wfm.Services.Features
{
    public partial class FeatureService : IFeatureService
    {
        #region Fields

        private readonly IRepository<Feature> _featureRepository;

        #endregion 


        #region Ctor

        public FeatureService(IRepository<Feature> featureRepository)
        {
            _featureRepository = featureRepository;
        }

        #endregion


        #region CRUD

        public void InsertFeature(Feature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature");

            _featureRepository.Insert(feature);
        }

        public void UpdateFeature(Feature feature)
        {
            if (feature == null)
                throw new ArgumentException("feature");

            _featureRepository.Update(feature);
        }

        public void DeleteFeature(Feature feature)
        {
            if (feature == null)
                throw new ArgumentException("feature");

            _featureRepository.Delete(feature);
        }

        #endregion


        #region Feature

        public Feature GetFeatureById(int id)
        {
            if (id == 0)
                return null;

            var query = _featureRepository.Table;

            query = query.Where(x => x.Id == id);

            return query.FirstOrDefault();
        }

        public IList<Feature> GetFeaturesByAreaAndNameAndCode(string area, string code, string name)
        {
            if (String.IsNullOrWhiteSpace(area) || String.IsNullOrWhiteSpace(code) || String.IsNullOrWhiteSpace(name))
                return null;

            var query = _featureRepository.Table;

            query = query.Where(x => x.Area == area && (x.Code == code || x.Name == name));

            return query.ToList();
        }

        public bool IsFeatureEnabled(string area, string name)
        {
            return _featureRepository.TableNoTracking.Any(x => x.IsActive && x.Area == area && x.Name == name);
        }

        #endregion


        #region LIST

        public IQueryable<Feature> GetAllFeatures(bool activeOnly = true)
        {
            var query = _featureRepository.Table;

            if (activeOnly)
                query = query.Where(x => x.IsActive);

            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            return query;
        }

        #endregion
    }
}
