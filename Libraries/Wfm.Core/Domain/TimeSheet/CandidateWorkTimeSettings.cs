using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.TimeSheet
{
    public class CandidateWorkTimeSettings : ISettings
    {
        public int StartScanWindowSpanInMinutes { get; set; }
        public int EndScanWindowSpanInMinutes { get; set; }

        public decimal MealBreakThreshold { get; set; }

        public decimal ValidWorkTimeRatio { get; set; }

        public int MatchBeforeStartTimeInMinutes { get; set; }
        public int MatchAfterStartTimeInMinutes { get; set; }
        public int MatchBeforeEndTimeInMinutes { get; set; }
        public int MatchAfterEndTimeInMinutes { get; set; }

        // PendingApprovalReminder
        public int PendingApprovalReminderDay { get; set; }
        public string PendingApprovalReminderTime { get; set; }
        public int PendingApprovalDueDay { get; set; }
        public string PendingApprovalDueTime { get; set; }

        public string WinnersRegularPayType { get; set; }
        public string WinnersAfternoonPayType { get; set; }
        public string WinnersNightPayType { get; set; }
        public string WinnersWeekendPayType { get; set; }
        public string WinnersDepartmentsWithShippingPremium { get; set; }
        public string WinnersDepartmentsWithSortationPremium { get; set; }

        public string CompaniesToImportEmployeeNumber { get; set; }
    }

}
