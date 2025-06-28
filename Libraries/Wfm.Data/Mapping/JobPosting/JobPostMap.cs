using System.Data.Entity.ModelConfiguration;
using JP = Wfm.Core.Domain.JobPosting;

namespace Wfm.Data.Mapping.JobPosting
{
    public partial class JobPostingMap : EntityTypeConfiguration<JP.JobPosting>
    {
        public JobPostingMap()
        {
            this.ToTable("JobPosting");
            this.HasKey(jp => jp.Id);
            this.HasRequired(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            this.HasRequired(x => x.CompanyLocation).WithMany().HasForeignKey(x => x.CompanyLocationId);
            this.HasOptional(x => x.CompanyDepartment).WithMany().HasForeignKey(x => x.CompanyDepartmentId);
            //this.HasRequired(x => x.CompanyContact).WithMany().HasForeignKey(x => x.CompanyContactId);
            this.HasRequired(x => x.JobType).WithMany().HasForeignKey(x => x.JobTypeId);
            this.HasRequired(x => x.JobPostingStatus).WithMany().HasForeignKey(x => x.JobPostingStatusId);
            this.HasRequired(x => x.JobCategory).WithMany().HasForeignKey(x => x.JobCategoryId);
            this.HasRequired(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId);
            this.HasRequired(x => x.Franchise).WithMany().HasForeignKey(x => x.FranchiseId);
            this.HasOptional(x => x.Account).WithMany().HasForeignKey(x => x.SubmittedBy);
            
        }
    }
}
