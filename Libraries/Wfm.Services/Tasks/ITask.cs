
using System.Threading.Tasks;
namespace Wfm.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface IScheduledTask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        void Execute();

        Task ExecuteAsync();
    }
}
