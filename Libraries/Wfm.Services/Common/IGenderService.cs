using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface IGenderService
    {
        /// <summary>
        /// Inserts the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        void InsertGender(Gender gender);

        /// <summary>
        /// Updates the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        void UpdateGender(Gender gender);

        /// <summary>
        /// Deletes the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        void DeleteGender(Gender gender);



        /// <summary>
        /// Gets the gender by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Gender GetGenderById(int id);

        /// <summary>
        /// Gets all genders. Use to save into DropDownList
        /// </summary>
        /// <returns></returns>
        List<Gender> GetAllGenders(bool showInactive = false, bool showHidden = false);

        int GetGenderIdByName(string name);

    }
}
