using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Policies
{
    public class PasswordPolicy:BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PasswordPolicyGuid { get; set; }
        public string Code { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool RequireUpperCase { get; set; }
        public bool RequireLowerCase { get; set; }
        public bool RequireNumber { get; set; }
        public bool RequireSymbol { get; set; }
        public int PasswordLifeTime { get; set; }
        public int PasswordHistory { get; set; }
    }
}
