using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Companies;


namespace Wfm.Core.Domain.Common
{
    
    public class Position
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PositionGuid { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
