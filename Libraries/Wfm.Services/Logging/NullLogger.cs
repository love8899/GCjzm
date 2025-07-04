using System;
using System.Linq;
using System.Collections.Generic;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Logging;

namespace Wfm.Services.Logging
{
    /// <summary>
    /// Null logger
    /// </summary>
    public partial class NullLogger : ILogger
    {
        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        public virtual bool IsEnabled(LogLevel level)
        {
            return false;
        }

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        public virtual void DeleteLog(Log log)
        {
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public virtual void ClearLog()
        {
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Log item collection</returns>
        public virtual IPagedList<Log> GetAllLogs(DateTime? fromUtc, DateTime? toUtc,
            string message, LogLevel? logLevel, int pageIndex, int pageSize)
        {
            return new PagedList<Log>(new List<Log>(), pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all logs asynchronous queryable.
        /// </summary>
        /// <param name="fromUtc">From UTC.</param>
        /// <param name="toUtc">The automatic UTC.</param>
        /// <param name="message">The message.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public IQueryable<Log> GetAllLogsAsQueryable(DateTime? fromUtc, DateTime? toUtc,
            string message, LogLevel? logLevel, int pageIndex, int pageSize)
        {
            return new List<Log>().AsQueryable();
        }

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        public virtual Log GetLogById(int logId)
        {
            return null;
        }

        /// <summary>
        /// Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        public virtual IList<Log> GetLogByIds(int[] logIds)
        {
            return new List<Log>();
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="customer">The customer to associate log record with</param>
        /// <returns>A log item</returns>
        public virtual Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Account account = null, string UserAgent = null)
        {
            return null;
        }
    }
}
