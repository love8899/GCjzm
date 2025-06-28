using System;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;


namespace Wfm.Services.Candidates
{
    public partial class UpdateCandidateStatusTask : IScheduledTask
    {
        #region Fields

        private readonly ICandidateService _candidateService;
        private readonly IRepository<CandidateBlacklist> _candidateBlacklistRepository;
        private readonly ILogger _logger;
        

        #endregion

        #region Ctor

        public UpdateCandidateStatusTask(
            ICandidateService candidateService,
            IRepository<CandidateBlacklist> candidateBlacklistRepository,
            ILogger logger)
        {
            _candidateService = candidateService;
            _candidateBlacklistRepository = candidateBlacklistRepository;
            _logger = logger;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                var today = DateTime.Today;
                var dueBlacklists = _candidateBlacklistRepository.Table.Where(x => !x.ClientId.HasValue && x.EffectiveDate <= today).ToList();

                foreach (var c in dueBlacklists)
                {
                    var candidate = _candidateService.GetCandidateById(c.CandidateId);
                    if (candidate != null && !candidate.IsBanned)
                        _candidateService.SetCandidateToBannedStatus(candidate, c.BannedReason);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while banning candidate with Id {0}", exc.Message), exc);
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }

    }

}
