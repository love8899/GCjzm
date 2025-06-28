using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface IAddressTypeService
    {

        /// <summary>
        /// Inserts the addressType.
        /// </summary>
        /// <param name="addressType">The addressType.</param>
        void InsertAddressType(AddressType addressType);

        /// <summary>
        /// Updates the addressType.
        /// </summary>
        /// <param name="addressType">The addressType.</param>
        void UpdateAddressType(AddressType addressType);

        /// <summary>
        /// Deletes the addressType.
        /// </summary>
        /// <param name="addressType">The addressType.</param>
        void DeleteAddressType(AddressType addressType);

        /// <summary>
        /// Gets the addressType by addressType id.
        /// </summary>
        /// <param name="addressTypeId">The addressType id.</param>
        /// <returns></returns>
        AddressType GetAddressTypeById(int id);

        int GetAddressTypeIdByName(string name);

        /// <summary>
        /// Gets all addressTypes.
        /// </summary>
        /// <returns></returns>
        IList<AddressType> GetAllAddressTypes(bool showInactive = false, bool showHidden = false);

    }
}
