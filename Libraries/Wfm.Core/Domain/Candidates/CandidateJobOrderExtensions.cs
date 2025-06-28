using System;
using System.Linq;
using Wfm.Core.Domain.Accounts;


namespace Wfm.Core.Domain.Candidates
{
    public static class CandidateJobOrderExtensions
    {
        public static IQueryable<CandidateJobOrder> FilterByClientAccount(this IQueryable<CandidateJobOrder> source, Account account)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (account == null)
                throw new ArgumentNullException("account");

            var query = source.Where(cjo => cjo.JobOrder.CompanyId == account.CompanyId);

            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) {; }

            else if (account.IsCompanyLocationManager())
                query = query.Where(cjo => cjo.JobOrder.CompanyLocationId > 0 && 
                                           cjo.JobOrder.CompanyLocationId == account.CompanyLocationId);

            else if (account.IsCompanyDepartmentManager() || account.IsCompanyDepartmentSupervisor())
            {
                query = query.Where(cjo => cjo.JobOrder.CompanyLocationId > 0 && 
                                           cjo.JobOrder.CompanyLocationId == account.CompanyLocationId &&
                                           cjo.JobOrder.CompanyDepartmentId > 0 && 
                                           cjo.JobOrder.CompanyDepartmentId == account.CompanyDepartmentId);

                if (account.IsCompanyDepartmentSupervisor())
                    query = query.Where(cjo => cjo.JobOrder.CompanyContactId > 0 &&
                                               cjo.JobOrder.CompanyContactId == account.Id);
            }

            return query;
        }
    }
}
