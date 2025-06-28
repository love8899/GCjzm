using System.Threading.Tasks;
using Wfm.Core.Caching;
using Wfm.Services.Tasks;

namespace Wfm.Services.Caching
{
    /// <summary>
    /// Clear cache schedueled task implementation
    /// </summary>
    public partial class ClearCacheTask : IScheduledTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();
        }

        public async Task ExecuteAsync()
        {
            await Task.Run( () => this.Execute() );
        }
    }
}
