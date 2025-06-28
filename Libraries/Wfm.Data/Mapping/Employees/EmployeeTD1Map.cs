using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Employees;


namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeTD1Map : EntityTypeConfiguration<EmployeeTD1>
    {
        public EmployeeTD1Map()
        {
            this.ToTable("Candidate_TD1");
            this.HasKey(x => x.Id);

            this.Property(x => x.Province_Code).HasMaxLength(2);

            this.Property(x => x.Basic_Amount).HasPrecision(10, 2);
            this.Property(x => x.Child_Amount).HasPrecision(10, 2);
            this.Property(x => x.Age_Amount).HasPrecision(10, 2);
            this.Property(x => x.Pension_Income_Amount).HasPrecision(10, 2);
            this.Property(x => x.Tuition_Amounts).HasPrecision(10, 2);
            this.Property(x => x.Disablility_Amount).HasPrecision(10, 2);
            this.Property(x => x.Spouse_Amount).HasPrecision(10, 2);
            this.Property(x => x.Eligible_Dependant_Amount).HasPrecision(10, 2);
            this.Property(x => x.Caregiver_Amount).HasPrecision(10, 2);
            this.Property(x => x.Infirm_Dependant_Amount).HasPrecision(10, 2);
            this.Property(x => x.Amount_Transferred_From_Spouse).HasPrecision(10, 2);
            this.Property(x => x.Amount_Transferred_From_Dependant).HasPrecision(10, 2);
            this.Property(x => x.Family_Tax_Benefit).HasPrecision(10, 2);
            this.Property(x => x.Senior_Supplementary_Amount).HasPrecision(10, 2);
            this.Property(x => x.Amount_For_Workers_65_Or_Older).HasPrecision(10, 2);
            this.Property(x => x.QC_Deductions).HasPrecision(10, 2);
            this.Property(x => x.TotalCredit).HasPrecision(25, 2);

            this.HasRequired(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId);
        }
    }
}
