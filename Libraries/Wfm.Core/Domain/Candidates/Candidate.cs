using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Incident;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.WSIB;


namespace Wfm.Core.Domain.Candidates
{
    public class Candidate : BaseEntity
    {
        private ICollection<CandidatePicture> _candidatePictures;
        private ICollection<CandidateJobOrder> _candidateJobOrders;
        private ICollection<CandidateWorkHistory> _candidateWorkHistories;
        private ICollection<CandidateKeySkill> _candidateKeySkills;
        private ICollection<CandidateAddress> _candidateAddresses;
        private ICollection<CandidateAttachment> _candidateAttachments;
        private ICollection<CandidateTestResult> _candidateTestResults;
        private ICollection<Alerts> _candidateAlerts;
        private ICollection<IncidentReport> _incidentReports;
        private ICollection<CandidateSmartCard> _smartCards;
        private ICollection<CandidateAppliedJobs> _appliedJobs;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CandidateGuid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PasswordFormatId { get; set; }
        public PasswordFormat PasswordFormat
        {
            get { return (PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }
        public string PasswordSalt { get; set; }
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
        public DateTime? SINExtensionSubmissionDate { get; set; }
        public string WorkPermit { get; set; }
        public DateTime? WorkPermitExpiry { get; set; }

        public string WebSite { get; set; }
        public string BestTimetoCall { get; set; }
        public bool DisabilityStatus { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
        public string InactiveReason { get; set; }
        public bool IsBanned { get; set; }
        public string BannedReason { get; set; }
        public bool IsDeleted { get; set; }

        public int? LanguageId { get; set; }

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

        public string LastIpAddress { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? LastActivityDateUtc { get; set; }

        public int EnteredBy { get; set; }
        public int OwnerId { get; set; }
        public int FranchiseId { get; set; }

        public string SearchKeys { get; set; }

        public int? EmployeeTypeId { get; set; }

        public string PayrollReminder { get; set; }

        public int? SecurityQuestion1Id { get; set; }
        public int? SecurityQuestion2Id { get; set; }

        public string SecurityQuestion1Answer { get; set; }
        public string SecurityQuestion2Answer { get; set; } 

        public string SecurityQuestionSalt { get; set; } 

        public int FailedSecurityQuestionAttempts {get;set;}
        public int SecurityAnswerFormatId { get; set; }

        public DateTime LastPasswordUpdateDate { get; set; }
        public bool UseForDirectPlacement { get; set; }

        public virtual Salutation Salutation { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual EthnicType EthnicType { get; set; }
        public virtual VetranType VetranType { get; set; }

        public virtual Source Source { get; set; }
        public virtual Transportation Transportation { get; set; }
        public virtual Shift Shift { get; set; }

        public virtual SecurityQuestion SecurityQuestion1 { get; set; }
        public virtual SecurityQuestion SecurityQuestion2 { get; set; }
       
        /// <summary>
        /// Gets or sets the collection of CandidatePicture
        /// </summary>
        public virtual ICollection<CandidatePicture> CandidatePictures
        {
            get { return _candidatePictures ?? (_candidatePictures = new List<CandidatePicture>()); }
            protected set { _candidatePictures = value; }
        }


        public virtual ICollection<CandidateJobOrder> CandidateJobOrders
        {
            get { return _candidateJobOrders ?? (_candidateJobOrders = new List<CandidateJobOrder>()); }
            protected set { _candidateJobOrders = value; }
        }

        public virtual ICollection<CandidateWorkHistory> CandidateWorkHistories
        {
            get { return _candidateWorkHistories ?? (_candidateWorkHistories = new List<CandidateWorkHistory>()); }
            protected set { _candidateWorkHistories = value; }
        }

        public virtual ICollection<CandidateKeySkill> CandidateKeySkills
        {
            get { return _candidateKeySkills ?? (_candidateKeySkills = new List<CandidateKeySkill>()); }
            protected set { _candidateKeySkills = value; }
        }

        public virtual ICollection<CandidateAddress> CandidateAddresses
        {
            get { return _candidateAddresses ?? (_candidateAddresses = new List<CandidateAddress>()); }
            protected set { _candidateAddresses = value; }
        }

        public virtual ICollection<CandidateAttachment> CandidateAttachments
        {
            get { return _candidateAttachments ?? (_candidateAttachments = new List<CandidateAttachment>()); }
            protected set { _candidateAttachments = value; }
        }

        public virtual ICollection<CandidateTestResult> CandidateTestResults
        {
            get { return _candidateTestResults ?? (_candidateTestResults = new List<CandidateTestResult>()); }
            protected set { _candidateTestResults = value; }
        }

        public virtual ICollection<Alerts> CandidateAlerts
        {
            get { return _candidateAlerts ?? (_candidateAlerts = new List<Alerts>()); }
            protected set { _candidateAlerts = value; }
        }

        public virtual ICollection<IncidentReport> IncidentReports
        {
            get { return _incidentReports ?? (_incidentReports = new List<IncidentReport>()); }
            protected set { _incidentReports = value; }
        }
        public virtual ICollection<CandidateSmartCard> SmartCards
        {
            get { return _smartCards ?? (_smartCards = new List<CandidateSmartCard>()); }
            protected set { _smartCards = value; }
        }
        public virtual ICollection<CandidateAppliedJobs> AppliedJobs
        {
            get { return _appliedJobs ?? (_appliedJobs = new List<CandidateAppliedJobs>()); }
            protected set { _appliedJobs = value; }
        }

        public virtual ICollection<CandidateBankAccount> CandidateBankAccounts { get; set; }
        public virtual ICollection<CandidateWSIBCommonRate> CandidateWSIBCommonRates { get; set; }
    }


  
}