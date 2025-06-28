using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public class AccountCompanyMap : EntityTypeConfiguration<RecruiterCompany>
    {
        public AccountCompanyMap()
        {
            this.ToTable("Account_Company_Mapping");
            this.HasKey(x => x.Id);
            this.HasRequired(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);
            this.HasRequired(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }
}
