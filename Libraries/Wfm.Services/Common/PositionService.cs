using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public class PositionService : IPositionService
    {
        #region Constants

        private const string POSITIONS_ALL_KEY = "Wfm.position.all-{0}";
        private const string POSITIONS_PATTERN_KEY = "Wfm.position.";

        #endregion

        #region Fields

        private readonly IRepository<Position> _positionRepository;
        private readonly ICacheManager _cacheManager;

        #endregion 

        #region Ctor

        public PositionService(ICacheManager cacheManager,
            IRepository<Position> positionRepository
            )
        {
            
            _cacheManager = cacheManager;
            _positionRepository = positionRepository;
        }

        #endregion 

        #region CRUD

        public void InsertPosition(Position position)
        {
            if (position == null)
                throw new ArgumentNullException("position");

            _positionRepository.Insert(position);

            //cache
            _cacheManager.RemoveByPattern(POSITIONS_PATTERN_KEY);
        }

        public void UpdatePosition(Position position)
        {
            if (position == null)
                throw new ArgumentNullException("position");

            _positionRepository.Update(position);

            //cache
            _cacheManager.RemoveByPattern(POSITIONS_PATTERN_KEY);
        }

        public void DeletePosition(Position position)
        {
            if (position == null)
                throw new ArgumentNullException("position");
            _positionRepository.Delete(position);

            //cache
            _cacheManager.RemoveByPattern(POSITIONS_PATTERN_KEY);
        }

        #endregion

        #region Position

        public Position GetPositionById(int? id)
        {
            if (id==null||id == 0)
                return null;

            return _positionRepository.GetById(id);
        }

        public int GetPositionIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var position = _positionRepository.Table.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();

            return position != null ? position.Id : 0;
        }

        public Position GetPositionByGuid(Guid guid)
        {
            var position = _positionRepository.Table.Where(x => x.PositionGuid == guid).FirstOrDefault();
            return position;
        }

        #endregion

        #region LIST

        public IList<Position> GetAllPositions()
        {
            //using cache
            //-----------------------------
            string key = string.Format(POSITIONS_ALL_KEY, true);
            return _cacheManager.Get(key, () =>
            {
                var query = _positionRepository.Table;

                query = from p in query
                        orderby p.Name
                        select p;

                return query.ToList();
            });
        }

        public IList<Position> GetAllPositionByCompanyId(int companyId)
        {
            string key = string.Format(POSITIONS_ALL_KEY, true);
            return _cacheManager.Get(key, () =>
            {
                var query = _positionRepository.Table;

                query = from p in query
                        orderby p.Name
                        where p.CompanyId==companyId
                        select p;

                return query.ToList();
            });
        }

        public IList<Position> GetAllPositionByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return new List<Position>();
            return _positionRepository.Table.Where(x => x.Company.CompanyGuid == guid).ToList();
        }

        public void DeleteAllPositionsByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var positions = _positionRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (positions.Count() > 0)
                _positionRepository.Delete(positions);
        }
        #endregion

    }
}
