using System;

namespace Wfm.Admin.Models.Companies
{
    public class CompanyCandidateViewModel
    {
        public int Id { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public int CompanyId { get; set; }

        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string HomePhone { get; set; }

        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }


        public string Note { get; set; }

        public DateTime? StartDate { get; set; }

        public int? RatingValue { get; set; }

        public DateTime? EndDate { get; set; }
        public string ReasonForLeave { get; set; }
        public string Status { get; set; }

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

        public string GenderName { get; set; }
      
        public string Intersection { get; set; }
        public string PreferredWorkLocation { get; set; }

        public string TransportationName { get; set; }
        public string ShiftName { get; set; }
        //public virtual TransportationModel TransportationModel { get; set; }
        //public virtual ShiftModel ShiftModel { get; set; }
    }
}