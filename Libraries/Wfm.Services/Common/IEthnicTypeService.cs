using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface IEthnicTypeService
    {
        /// <summary>
        /// Inserts the ethnicType.
        /// </summary>
        /// <param name="ethnicType">The ethnicType.</param>
        void InsertEthnicType(EthnicType ethnicType);

        /// <summary>
        /// Updates the ethnicType.
        /// </summary>
        /// <param name="ethnicType">The ethnicType.</param>
        void UpdateEthnicType(EthnicType ethnicType);

        /// <summary>
        /// Deletes the ethnicType.
        /// </summary>
        /// <param name="ethnicType">The ethnicType.</param>
        void DeleteEthnicType(EthnicType ethnicType);

        /// <summary>
        /// Gets the ethnicType by ethnicType id.
        /// </summary>
        /// <param name="id">The ethnicType id.</param>
        /// <returns></returns>
        EthnicType GetEthnicTypeById(int id);

        /// <summary>
        /// Gets all ethnicTypes.
        /// </summary>
        /// <returns></returns>
        IList<EthnicType> GetAllEthnicTypes(bool showInactive = false, bool showHidden = false);

    }
}
