namespace Wfm.Core.Domain.Common
{
    public class City : BaseEntity
    {
        public int StateProvinceId { get; set; }
        public string CityName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual StateProvince StateProvince { get; set; }
    }
}