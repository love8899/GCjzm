using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface ITransportationService
    {
        /// <summary>
        /// Inserts the transportation.
        /// </summary>
        /// <param name="transportation">The transportation.</param>
        void InsertTransportation(Transportation transportation);

        /// <summary>
        /// Updates the transportation.
        /// </summary>
        /// <param name="transportation">The transportation.</param>
        void UpdateTransportation(Transportation transportation);

        /// <summary>
        /// Deletes the transportation.
        /// </summary>
        /// <param name="transportation">The transportation.</param>
        void DeleteTransportation(Transportation transportation);



        /// <summary>
        /// Gets the transportation by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Transportation GetTransportationById(int id);

        int GetTransportationIdByName(string name);

        /// <summary>
        /// Gets all transportations. Use to save into DropDownList
        /// </summary>
        /// <returns></returns>
        List<Transportation> GetAllTransportations(bool showInactive = false, bool showHidden = false);

    }
}
