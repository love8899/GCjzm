using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface IIntersectionService
    {
        /// <summary>
        /// Inserts the intersection.
        /// </summary>
        /// <param name="intersection">The intersection.</param>
        void InsertIntersection(Intersection intersection);

        /// <summary>
        /// Updates the intersection.
        /// </summary>
        /// <param name="intersection">The intersection.</param>
        void UpdateIntersection(Intersection intersection);

        /// <summary>
        /// Deletes the intersection.
        /// </summary>
        /// <param name="intersection">The intersection.</param>
        void DeleteIntersection(Intersection intersection);



        /// <summary>
        /// Gets the intersection by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Intersection GetIntersectionById(int id);

        int GetIntersectionIdByName(string name);

        /// <summary>
        /// Gets all intersections. Use to save into DropDownList
        /// </summary>
        /// <returns></returns>
        List<Intersection> GetAllIntersections(bool showInactive = false, bool showHidden = false);

    }
}
