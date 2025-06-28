using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Logging;

namespace Wfm.Services.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogger logger, string message, Exception exception = null, Account account = null, string userAgent = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, account, userAgent);
        }
        public static void Information(this ILogger logger, string message, Exception exception = null, Account account = null, string userAgent = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, account, userAgent);
        }
        public static void Warning(this ILogger logger, string message, Exception exception = null, Account account = null, string userAgent = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, account, userAgent);
        }
        public static void Error(this ILogger logger, string message, Exception exception = null, Account account = null, string userAgent = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, account, userAgent);
        }
        public static void Fatal(this ILogger logger, string message, Exception exception = null, Account account = null, string userAgent =  null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, account, userAgent);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null, Account account = null, string userAgent = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (logger.IsEnabled(level))
            {
                string fullMessage = exception == null ? string.Empty : exception.ToString();
                logger.InsertLog(level, message, fullMessage, account, userAgent);
            }
        }
    }
}
