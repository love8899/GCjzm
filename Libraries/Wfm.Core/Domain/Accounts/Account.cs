using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Core.Domain.Accounts
{
    public class Account : BaseEntity
    {
        //private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;
        private ICollection<AccountRole> _accountRoles;
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AccountGuid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PasswordFormatId { get; set; }
        public PasswordFormat PasswordFormat
        {
            get { return (PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string WorkPhone { get; set; }
        public int CompanyId { get; set; }
        public int CompanyLocationId { get; set; }
        public int CompanyDepartmentId { get; set; }
        public int ManagerId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClientAccount { get; set; }
        
        public bool IsSystemAccount { get; set; }
        public string SystemName { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? LastActivityDateUtc { get; set; }
        public int EnteredBy { get; set; }
        public int FranchiseId { get; set; }
        public bool IsLimitedToFranchises { get; set; }

        public string FullName { get { return String.Concat(FirstName , " " , LastName).Trim(); } }

        public int? SecurityQuestion1Id { get; set; }
        public int? SecurityQuestion2Id { get; set; }

        public string SecurityQuestion1Answer { get; set; }
        public string SecurityQuestion2Answer { get; set; }

        public string SecurityQuestionSalt { get; set; }
        public int FailedSecurityQuestionAttempts { get; set; }
        public int? ShiftId { get; set; }

        public DateTime LastPasswordUpdateDate { get; set; }

        public int? EmployeeId { get; set; }
         
        #region Navigation properties
        
        public virtual SecurityQuestion SecurityQuestion1 { get; set; }
        public virtual SecurityQuestion SecurityQuestion2 { get; set; }

        public virtual Franchise Franchise { get; set; }
        public virtual Shift Shift { get; set; }
        /// <summary>
        /// Gets or sets account company
        /// </summary>
        //Use loose relation for better intergration
        //--------------------------------------------
        //public virtual Company Company { get; set; }

        /// <summary>
        /// Gets or sets account generated content
        /// </summary>
        //public virtual ICollection<ExternalAuthenticationRecord> ExternalAuthenticationRecords
        //{
        //    get { return _externalAuthenticationRecords ?? (_externalAuthenticationRecords = new List<ExternalAuthenticationRecord>()); }
        //    protected set { _externalAuthenticationRecords = value; }
        //}

        /// <summary>
        /// Gets or sets the account roles
        /// </summary>
        public virtual ICollection<AccountRole> AccountRoles
        {
            get { return _accountRoles ?? (_accountRoles = new List<AccountRole>()); }
            protected set { _accountRoles = value; }
        }


        #endregion
    }
}
