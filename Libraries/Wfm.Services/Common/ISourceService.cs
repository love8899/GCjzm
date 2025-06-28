using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface ISourceService
    {
        /// <summary>
        /// Inserts the source.
        /// </summary>
        /// <param name="source">The source.</param>
        void InsertSource(Source source);

        /// <summary>
        /// Updates the source.
        /// </summary>
        /// <param name="source">The source.</param>
        void UpdateSource(Source source);

        /// <summary>
        /// Deletes the source.
        /// </summary>
        /// <param name="source">The source.</param>
        void DeleteSource(Source source);



        /// <summary>
        /// Gets the source by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Source GetSourceById(int id);

        int GetSourceIdByName(string name);

        /// <summary>
        /// Gets all sources. Use to save into DropDownList
        /// </summary>
        /// <returns></returns>
        List<Source> GetAllSources(bool showInactive = false, bool showHidden = false);

    }
}
