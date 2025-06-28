using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface ISkillService
    {
        /// <summary>
        /// Inserts the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void InsertSkill(Skill skill);

        /// <summary>
        /// Updates the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void UpdateSkill(Skill skill);

        /// <summary>
        /// Deletes the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void DeleteSkill(Skill skill);



        /// <summary>
        /// Gets the skill by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Skill GetSkillById(int id);

        int GetSkillIdByName(string name);

        /// <summary>
        /// Gets all skills. Use to save into DropDownList
        /// </summary>
        /// <returns></returns>
        List<Skill> GetAllSkills(bool showInactive = false, bool showHidden = false);

    }
}
