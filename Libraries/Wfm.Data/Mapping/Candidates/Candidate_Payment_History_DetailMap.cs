using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public class Candidate_Payment_History_DetailMap : EntityTypeConfiguration<Candidate_Payment_History_Detail>
    {
        public Candidate_Payment_History_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Candidate_Payment_History_Detail");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Payment_HistoryId).HasColumnName("Payment_HistoryId");
            this.Property(t => t.Payroll_ItemId).HasColumnName("Payroll_ItemId");
            this.Property(t => t.Unit).HasColumnName("Unit");
            this.Property(t => t.Rate).HasColumnName("Rate");
            this.Property(t => t.Amount).HasColumnName("Amount");
           // this.Property(t => t.YTD_Unit).HasColumnName("YTD_Unit");
           // this.Property(t => t.YTD_Amount).HasColumnName("YTD_Amount");
            this.Property(t => t.JobOrder_Id).HasColumnName("JobOrder_Id");

            // Relationships
            this.HasRequired(t => t.Candidate_Payment_History)
                .WithMany(t => t.Candidate_Payment_History_Detail)
                .HasForeignKey(d => d.Payment_HistoryId);
          //  this.HasRequired(t => t.Payroll_Item)
          //      .WithMany(t => t.Candidate_Payment_History_Detail)
          //      .HasForeignKey(d => d.Payroll_ItemId);

        }
    }
}
