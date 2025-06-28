using System;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Candidates;


namespace Wfm.Core.Domain.Payroll
{
    public abstract class T4_Base
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FranchiseId { get; set; }

        public int CandidateId { get; set; }

        [NotMapped]
        public Guid CandidateGuid { get; set; }

        public string LastName { get; set; }
        public string Initials { get; set; }
        public string FirstName { get; set; }
        
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Postalcode { get; set; }
        public string ProvinceCode { get; set; }
        public string CountryCode { get; set; }

        public string SocialInsuranceNumber { get; set; }

        public string EmployeeNumber { get; set; }
        public decimal EmploymentIncome { get; set; }
        public decimal CPP { get; set; }
        public decimal QPP { get; set; }
        public decimal EIPremium { get; set; }
        public decimal RPP { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal InsurableEarnings { get; set; }
        public decimal PensionableEarnings { get; set; }

        public byte ExemptCPP_QPP { get; set; }
        public byte ExemptEI { get; set; }
        public byte? ExemptPPIP { get; set; }

        public string EmploymentCode { get; set; }

        public decimal UnionPay { get; set; }
        public decimal CharitableDonations { get; set; }
        public string RPP_DPSPNumber { get; set; }
        public decimal PensionAdjustment { get; set; }
        public decimal PPIPPremiums { get; set; }
        public decimal PPIPInsurableEarnings { get; set; }

        public string OtherInfoBox1Code { get; set; }
        public string OtherInfoBox2Code { get; set; }
        public string OtherInfoBox3Code { get; set; }
        public string OtherInfoBox4Code { get; set; }
        public string OtherInfoBox5Code { get; set; }
        public string OtherInfoBox6Code { get; set; }
        public decimal? OtherInfoBox1Amount { get; set; }
        public decimal? OtherInfoBox2Amount { get; set; }
        public decimal? OtherInfoBox3Amount { get; set; }
        public decimal? OtherInfoBox4Amount { get; set; }
        public decimal? OtherInfoBox5Amount { get; set; }
        public decimal? OtherInfoBox6Amount { get; set; }

        public string ReportTypeCode { get; set; }
        public bool? IsSubmitted { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }

        public string Box10_ProvinceCode { get; set; }

        public decimal? EmployersCPP { get; set; }
        public decimal? EmployersQPP { get; set; }
        public decimal? EmployersEI { get; set; }
        public decimal? EmployersQPIP { get; set; }

        public int? SequentialNumber { get; set; }

        public bool EmailSent { get; set; }

        public virtual Candidate Candidate { get; set; }

        #endregion
    }
}
