using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateWithAddress 
    {

        private ICollection<CandidateAddress> _candidateAddresses;


        public int Id { get; set; }
        public Guid Guid { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }

        public string Username { get; set; }
 
        //public string PasswordSalt { get; set; }
        public string EmployeeId { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public int SalutationId { get; set; }
        public int GenderId { get; set; }
        public int? EthnicTypeId { get; set; }
        public int? VetranTypeId { get; set; }
        public int? SourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string EmergencyPhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public DateTime? SINExpiryDate { get; set; }

        public string WebSite { get; set; }
        public string BestTimetoCall { get; set; }
        public bool DisabilityStatus { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }

        public int CandidateOnboardingStatusId { get; set; }
        public bool IsEmployee { get; set; }

        public bool Entitled { get; set; }
        public bool CanRelocate { get; set; }
        public string JobTitle { get; set; }
        public string Education { get; set; }
        public string Education2 { get; set; }
        public DateTime? DateAvailable { get; set; }
        public string CurrentEmployer { get; set; }
        public string CurrentPay { get; set; }
        public string DesiredPay { get; set; }
        public int? ShiftId { get; set; }
        public int? TransportationId { get; set; }
        public string LicencePlate { get; set; }
        public string MajorIntersection1 { get; set; }
        public string MajorIntersection2 { get; set; }
        public string PreferredWorkLocation { get; set; }

        public string Note { get; set; }

       // public string LastIpAddress { get; set; }
      //  public DateTime? LastLoginDateUtc { get; set; }
     //   public DateTime? LastActivityDateUtc { get; set; }

        public int EnteredBy { get; set; }
        public int OwnerId { get; set; }
        public int FranchiseId { get; set; }



        public string SearchKeys { get; set; }





        public virtual Salutation Salutation { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual EthnicType EthnicType { get; set; }
        public virtual VetranType VetranType { get; set; }

        public virtual Source Source { get; set; }
        public virtual Transportation Transportation { get; set; }
        public virtual Shift Shift { get; set; }



        public string Age { get; set; }

        public int? CityId { get; set; }
        public int? StateProvinceId { get; set; }

        public bool OnBoarded { get; set; }
        public bool UseForDirectPlacement { get; set; }

        public virtual ICollection<CandidateAddress> CandidateAddresses
        {
            get { return _candidateAddresses ?? (_candidateAddresses = new List<CandidateAddress>()); }
            protected set { _candidateAddresses = value; }
        }

        public virtual DateTime? CreatedOn
        {
            get
            {
                DateTime dt = DateTime.MinValue;
                if (CreatedOnUtc.HasValue)
                {
                    dt = CreatedOnUtc.Value;
                }
                return dt.ToLocalTime();
            }

            set
            {
            }
        }

        public virtual DateTime? UpdatedOn
        {
            get
            {
                DateTime dt = DateTime.MinValue;
                if (UpdatedOnUtc.HasValue)
                {
                    dt = UpdatedOnUtc.Value;
                }
                return dt.ToLocalTime();
            }

            set
            {
            }
        }
    }
}
