using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface IVetranTypeService
    {

        /// <summary>
        /// Inserts the vetranType.
        /// </summary>
        /// <param name="vetranType">The vetranType.</param>
        void InsertVetranType(VetranType vetranType);

        /// <summary>
        /// Updates the vetranType.
        /// </summary>
        /// <param name="vetranType">The vetranType.</param>
        void UpdateVetranType(VetranType vetranType);

        /// <summary>
        /// Deletes the vetranType.
        /// </summary>
        /// <param name="vetranType">The vetranType.</param>
        void DeleteVetranType(VetranType vetranType);

        /// <summary>
        /// Gets the vetranType by vetranType id.
        /// </summary>
        /// <param name="vetranTypeId">The vetranType id.</param>
        /// <returns></returns>
        VetranType GetVetranTypeById(int id);

        /// <summary>
        /// Gets all vetranTypes.
        /// </summary>
        /// <returns></returns>
        IList<VetranType> GetAllVetranTypes(bool showInactive = false, bool showHidden = false);

    }
}
