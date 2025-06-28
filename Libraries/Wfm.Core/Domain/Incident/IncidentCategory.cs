using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Core.Domain.Incident
{
    /// <summary>
    /// Categoary of Incident
    /// </summary>
    public class IncidentCategory : BaseEntity
    {
        /// <summary>
        /// null for all franchise, 
        /// </summary>
        public int? FranchiseId { get; set; }
        public virtual Franchise Franchise { get; set; }
        /// <summary>
        /// Category enumeration
        /// </summary>
        public int IncidentCategoryCode { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Active Flag
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// All templates of this category
        /// </summary>
        public ICollection<IncidentReportTemplate> Templates { get; set; }
    }
    //public enum IncidentCategoryCodeEnum
    //{
    //    Injury = 1,
    //    Disability,
    //    Illness,
    //    Harassment,
    //    PropertyDamage,
    //    Other,
    //}
}
