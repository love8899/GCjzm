using System;
using System.ComponentModel.DataAnnotations.Schema;
using RecogSys.RdrAccess;
using Wfm.Core.Domain.Candidates;


namespace Wfm.Core.Domain.ClockTime
{
    public class HandTemplate : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid HandTemplateGuid { get; set; }

        public int CandidateId { get; set; }
        public byte[] TemplateVector { get; set; }
        public byte AuthorityLevel { get; set; }
        public byte RejectThreshold { get; set; }
        public byte TimeZone { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Note { get; set; }
        public int EnteredBy { get; set; }

        public virtual Candidate Candidate { get; set; }


        public RsiUserRecord ToRsiUserRecord(string smartCardUid = null)
        {
            var userRecord = new RsiUserRecord();
            userRecord.pID.SetID(smartCardUid ?? CandidateId.ToString());   // smart card ID or manual ID
            userRecord.pTemplateVector.Set(TemplateVector);
            userRecord.authorityLevel = (RSI_AUTHORITY_LEVEL)AuthorityLevel;
            userRecord.rejectThreshold = RejectThreshold;
            userRecord.timeZone = TimeZone;

            return userRecord;
        }
    }


    public class RsiUserRecord : RSI_USER_RECORD
    {
        public HandTemplate ToHandTemplate(string candidateIdStr = null)
        {
            return new HandTemplate()
            {
                CandidateId = Convert.ToInt32(candidateIdStr ?? pID.GetID()),   // manual ID or smart card ID
                TemplateVector = pTemplateVector.Get(),
                AuthorityLevel = (byte)authorityLevel,
                RejectThreshold = (byte)rejectThreshold,
                TimeZone = timeZone,
                IsActive = true,
            };
        }
    }
}
