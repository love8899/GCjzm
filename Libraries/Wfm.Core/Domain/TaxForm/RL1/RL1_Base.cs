using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Wfm.Core.Domain.TaxForm.RL1
{
    public abstract class RL1_Base
    {
        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FranchiseId { get; set; }    

        [NotMapped]
        public Guid CandidateGuid { get; set; }                         

        public int CandidateId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string SIN { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public int CityId { get; set; }

        public int StateProvinceId { get; set; }

        public int CountryId { get; set; }

        public string EmployerAddressLine1 { get; set; }

        public string EmployerAddressLine2 { get; set; }

        public string EmployerAddressLine3 { get; set; }

        public int EmployerCityId { get; set; }

        public int EmployerStateProvinceId { get; set; }

        public int EmployerCountryId { get; set; }

        public string Code { get; set; }

        public string SequentialNum { get; set; }

        public decimal? Box_A { get; set; }

        public decimal? Box_B { get; set; }

        public decimal? Box_C { get; set; }

        public decimal? Box_D { get; set; }

        public decimal? Box_E { get; set; }

        public decimal? Box_F { get; set; }

        public decimal? Box_G { get; set; }

        public decimal? Box_H { get; set; }

        public decimal? Box_I { get; set; }

        public decimal? Box_J { get; set; }

        public decimal? Box_K { get; set; }

        public decimal? Box_L { get; set; }

        public decimal? Box_M { get; set; }

        public decimal? Box_N { get; set; }

        public decimal? Box_O { get; set; }

        public decimal? Box_P { get; set; }

        public decimal? Box_Q { get; set; }

        public decimal? Box_R { get; set; }

        public decimal? Box_S { get; set; }

        public decimal? Box_T { get; set; }

        public decimal? Box_U { get; set; }

        public decimal? Box_V { get; set; }

        public decimal? Box_W { get; set; }

        public string Code_Case_O { get; set; }

        public string Additional_Info1_Code { get; set; }

        public string Additional_Info2_Code { get; set; }

        public string Additional_Info3_Code { get; set; }

        public string Additional_Info4_Code { get; set; }

        public int ReferenceNumber { get; set; }

        public bool? IsSubmitted { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public string PostalCode { get; set; }

        public string LastSequentialNum { get; set; }

        public decimal? Additional_Info1_Value { get; set; }

        public decimal? Additional_Info2_Value { get; set; }

        public decimal? Additional_Info3_Value { get; set; }

        public decimal? Additional_Info4_Value { get; set; }

        public string XMLSequentialNum { get; set; }

        public bool EmailSent { get; set; }

        public decimal? EmployersQPP { get; set; }

        public decimal? EmployersQPIP { get; set; }

        public decimal GrossPay { get; set; }

        public string LastXMLSeqNumber { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [NotMapped]
        public string EmployeeNumber { get; set; }

        #endregion
    }
}
