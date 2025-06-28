using System;

namespace Wfm.Admin.Models.Candidate
{
    public partial class CandidateProfile
    {
        public int[] CityIds { get; set; }

        public string Intersection { get; set; } 

        public int[] XportationIds { get; set; }

        public int[] ShiftIds { get; set; }

        public int[] SkillIds { get; set; }

        public int? MinEduLevel { get; set; }

        public int? GenderId { get; set; }

        public bool IsActive { get; set; }
        public bool Onboarded { get; set; }
        public bool IsEmployee { get; set; }

        public bool ByPlacement { get; set; }
        public bool IsPlaced { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public enum EducationLevel
    {
        Any = 1,
        HighSchool = 2,
        College = 3,
        University = 4,
        Graduate = 5,
    }

}
