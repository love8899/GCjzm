using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public partial class SkillService : ISkillService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string SKILLS_ALL_KEY = "Wfm.skill.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SKILLS_PATTERN_KEY = "Wfm.skill.";

        #endregion

        #region Fields

        private readonly IRepository<Skill> _skillRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public SkillService(ICacheManager cacheManager,
            IRepository<Skill> skillRepository)
        {
            _cacheManager = cacheManager;
            _skillRepository = skillRepository;
        }

        #endregion


        #region CRUD

        /// <summary>
        /// Inserts the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <exception cref="System.ArgumentNullException">skill is null</exception>
        public void InsertSkill(Skill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            _skillRepository.Insert(skill);

            //cache
            _cacheManager.RemoveByPattern(SKILLS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <exception cref="System.ArgumentNullException">skill</exception>
        public void UpdateSkill(Skill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            _skillRepository.Update(skill);

            //cache
            _cacheManager.RemoveByPattern(SKILLS_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <exception cref="System.ArgumentNullException">skill</exception>
        public void DeleteSkill(Skill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            skill.IsActive = false;
            _skillRepository.Update(skill);

            //cache
            _cacheManager.RemoveByPattern(SKILLS_PATTERN_KEY);
        }

        #endregion

        #region Skill

        /// <summary>
        /// Gets the skill by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Skill GetSkillById(int id)
        {
            if (id == 0)
                return null;

            return _skillRepository.GetById(id);
        }

        public int GetSkillIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var skill = _skillRepository.Table.Where(x => x.SkillName.ToLower() == name.ToLower()).FirstOrDefault();

            return skill != null ? skill.Id : 0;
        }

        #endregion

        #region LIST


        /// <summary>
        /// Gets all skills. Use to save into DropDownList
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public List<Skill> GetAllSkills(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(SKILLS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _skillRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.SkillName
                        select s;

                return query.ToList();
            });
        }


        #endregion

    }
}
