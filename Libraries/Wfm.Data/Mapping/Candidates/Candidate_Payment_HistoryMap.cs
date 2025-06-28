using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public class Candidate_Payment_HistoryMap : EntityTypeConfiguration<Candidate_Payment_History>
    {
        public Candidate_Payment_HistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Year)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.Cheque_Number)
                .HasMaxLength(20);

            this.Property(t => t.Direct_Deposit_Number)
                .HasMaxLength(20);

            this.Property(t => t.ProvinceCode)
                .HasMaxLength(2);

            // Table & Column Mappings
            this.ToTable("Candidate_Payment_History");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CandidateId).HasColumnName("CandidateId");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.Payment_Date).HasColumnName("Payment_Date");
            this.Property(t => t.Cheque_Number).HasColumnName("Cheque_Number");
            this.Property(t => t.Direct_Deposit_Number).HasColumnName("Direct_Deposit_Number");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
           // this.Property(t => t.EmployeeType).HasColumnName("EmployeeType");
            this.Property(t => t.PayrollBatchId).HasColumnName("PayrollBatchId");
            this.Property(t => t.ProvinceCode).HasColumnName("ProvinceCode");
           // this.Property(t => t.Paystub).HasColumnName("Paystub");
            this.Property(t => t.CheckStatusId).HasColumnName("CheckStatusId");
            this.Property(t => t.IsEmailed).HasColumnName("IsEmailed");
            this.Property(t => t.IsPrinted).HasColumnName("IsPrinted");

            // Relationships
            this.HasOptional(t => t.Check_Status)
                .WithMany(t => t.Candidate_Payment_History)
                .HasForeignKey(d => d.CheckStatusId);

        }
    }
}
