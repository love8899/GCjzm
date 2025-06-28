using System;
using Wfm.Core.Domain.Common;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateAddress : BaseEntity, ICloneable
    {
        public int CandidateId { get; set; }
        public int AddressTypeId { get; set; }
      
        public string UnitNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
        //public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

       

        public virtual Candidate Candidate { get; set; }
        public virtual AddressType AddressType { get; set; }


        public object Clone()
        {
            var addr = new CandidateAddress()
            {
                CandidateId = this.CandidateId,
                AddressTypeId = this.AddressTypeId,
                UnitNumber = this.UnitNumber,
                AddressLine1 = this.AddressLine1,
                AddressLine2 = this.AddressLine2,
                AddressLine3 = this.AddressLine3,
                CityId = this.CityId,
                StateProvinceId = this.StateProvinceId,
                CountryId = this.CountryId,
                PostalCode = this.PostalCode,
                //Note = this.Note,
                IsActive = this.IsActive,
                AddressType = this.AddressType,
            };
            return addr;
        }     

        public void CleanUp()
        {
            this.PostalCode = this.PostalCode.Replace(" ", "");
        }

    }


    public class CandidateAddressWithName 
    {

        public int Id { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }


        public int CandidateId { get; set; }
        public int FranchiseId { get; set; }
        public int AddressTypeId { get; set; }
     
        public string UnitNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }       
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
   
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }



        public virtual Candidate Candidate { get; set; }
        public virtual AddressType AddressType { get; set; }      

        public string FullAddress { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }

    }
}
