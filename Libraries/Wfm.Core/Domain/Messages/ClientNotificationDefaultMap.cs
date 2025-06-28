using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;


namespace Wfm.Core.Domain.Messages
{
    public static class ClientNotificationDefaultMap
    {
        public static void Create()
        {
            if (DefaultMap.Count <= 0)
            {
                DefaultMap.Add(
                    "CandidateMissedClocking.AccountNotification",
                    new string[]
                { 
                });

                DefaultMap.Add(
                    "JobOrderPlacementNotification",
                    new string[]
                { 
                    "CompanyDepartmentSupervisors",
                });

                DefaultMap.Add(
                    "JobPosting.PublishJobPosting",
                    new string[]
                { 
                    "CompanyDepartmentSupervisors",
                });

                DefaultMap.Add(
                    "JobPosting.SubmissionReminder",
                    new string[]
                { 
                    "CompanyDepartmentSupervisors",
                });

                DefaultMap.Add(
                    "TimeSheet.PendingApprovalReminder",
                    new string[]
                { 
                    "HrManagers",
                    "CompanyDepartmentSupervisors",
                });

                DefaultMap.Add(
                    "CandidateClockTime.UnexpectedPunchIn",
                    new string[]
                { 
                    "HrManagers",
                });
            }
        }
        
        public static IDictionary<string, string[]> DefaultMap = new Dictionary<string, string[]>();
    }
}
