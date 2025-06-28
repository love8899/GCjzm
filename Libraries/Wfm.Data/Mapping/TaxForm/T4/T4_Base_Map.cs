using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Data.Mapping.Payroll
{
    public abstract class T4_Base_Map<T> : EntityTypeConfiguration<T> where T : T4_Base
    {
        public T4_Base_Map()
        {
            this.HasKey(x => x.Id);

            this.Property(x => x.LastName).HasMaxLength(20);
            this.Property(x => x.Initials).HasMaxLength(1);
            //this.Property(x => x.FirstName).HasMaxLength(20);

            this.Property(x => x.AddressLine1).HasMaxLength(30);
            this.Property(x => x.AddressLine2).HasMaxLength(30);
            this.Property(x => x.City).HasMaxLength(28);
            this.Property(x => x.Postalcode).HasMaxLength(10);
            this.Property(x => x.ProvinceCode).HasMaxLength(2);
            this.Property(x => x.CountryCode).HasMaxLength(3);

            this.Property(x => x.SocialInsuranceNumber).HasMaxLength(9);

            this.Property(x => x.EmployeeNumber).HasMaxLength(20);

            this.Property(x => x.EmploymentIncome).HasPrecision(10, 2);
            this.Property(x => x.CPP).HasPrecision(6, 2);
            this.Property(x => x.QPP).HasPrecision(6, 2);
            this.Property(x => x.EIPremium).HasPrecision(6, 2);
            this.Property(x => x.RPP).HasPrecision(7, 2);
            this.Property(x => x.IncomeTax).HasPrecision(10, 2);
            this.Property(x => x.InsurableEarnings).HasPrecision(7, 2);

            this.Property(x => x.PensionableEarnings).HasPrecision(9, 2);

            this.Property(x => x.EmploymentCode).HasMaxLength(2);

            this.Property(x => x.UnionPay).HasPrecision(9, 2);
            this.Property(x => x.CharitableDonations).HasPrecision(9, 2);

            this.Property(x => x.RPP_DPSPNumber).HasMaxLength(7);

            this.Property(x => x.PensionAdjustment).HasPrecision(7, 2);
            this.Property(x => x.PPIPPremiums).HasPrecision(6, 2);
            this.Property(x => x.PPIPInsurableEarnings).HasPrecision(18, 2);

            this.Property(x => x.OtherInfoBox1Code).HasMaxLength(10);
            this.Property(x => x.OtherInfoBox2Code).HasMaxLength(10);
            this.Property(x => x.OtherInfoBox3Code).HasMaxLength(10);
            this.Property(x => x.OtherInfoBox4Code).HasMaxLength(10);
            this.Property(x => x.OtherInfoBox5Code).HasMaxLength(10);
            this.Property(x => x.OtherInfoBox6Code).HasMaxLength(10);

            this.Property(x => x.OtherInfoBox1Amount).HasPrecision(9, 2);
            this.Property(x => x.OtherInfoBox2Amount).HasPrecision(9, 2);
            this.Property(x => x.OtherInfoBox3Amount).HasPrecision(9, 2);
            this.Property(x => x.OtherInfoBox4Amount).HasPrecision(9, 2);
            this.Property(x => x.OtherInfoBox5Amount).HasPrecision(9, 2);
            this.Property(x => x.OtherInfoBox6Amount).HasPrecision(9, 2);

            this.Property(x => x.ReportTypeCode).HasMaxLength(1);

            this.Property(x => x.Box10_ProvinceCode).HasMaxLength(2);

            this.Property(x => x.EmployersCPP).HasPrecision(10, 2);
            this.Property(x => x.EmployersQPP).HasPrecision(10, 2);
            this.Property(x => x.EmployersEI).HasPrecision(10, 2);
            this.Property(x => x.EmployersQPIP).HasPrecision(10, 2);
        }
    }
}
