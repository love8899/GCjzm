using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface ISalutationService
    {
        /// <summary>
        /// Inserts the salutation.
        /// </summary>
        /// <param name="salutation">The salutation.</param>
        void InsertSalutation(Salutation salutation);

        /// <summary>
        /// Updates the salutation.
        /// </summary>
        /// <param name="salutation">The salutation.</param>
        void UpdateSalutation(Salutation salutation);

        /// <summary>
        /// Deletes the salutation.
        /// </summary>
        /// <param name="salutation">The salutation.</param>
        void DeleteSalutation(Salutation salutation);



        /// <summary>
        /// Gets the salutation by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Salutation GetSalutationById(int id);

        int GetSalutationIdByName(string name);

        /// <summary>
        /// Gets all salutations. Use to save into DropDownList
        /// </summary>
        /// <returns></returns>
        List<Salutation> GetAllSalutations(bool showInactive = false, bool showHidden = false);

    }
}
