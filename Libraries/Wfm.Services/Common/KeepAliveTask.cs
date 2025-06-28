using System;
using System.Net;
using System.Threading.Tasks;
using Wfm.Core.Domain.Common;
using Wfm.Services.Tasks;

namespace Wfm.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : IScheduledTask
    {
        private readonly CommonSettings _commonSettings;

        public KeepAliveTask(
            CommonSettings commonSettings)
        {
            _commonSettings = commonSettings;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            string host = _commonSettings.HostUrl;

            using (var wc = new WebClient())
            {
                wc.DownloadString(String.Concat(host , "/keepalive/index"));
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}
