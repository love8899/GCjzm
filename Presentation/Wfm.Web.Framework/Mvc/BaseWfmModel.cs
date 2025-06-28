using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Wfm.Web.Framework.Mvc
{
    /// <summary>
    /// Base wfmCommerce model
    /// </summary>
    [ModelBinder(typeof(WfmModelBinder))]
    public partial class BaseWfmModel
    {
        public BaseWfmModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }

        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
    }

    /// <summary>
    /// Base wfmCommerce entity model
    /// </summary>
    public partial class BaseWfmEntityModel : BaseWfmModel
    {
        [WfmResourceDisplayName("Common.Id")]
        public virtual int Id { get; set; }



        [WfmResourceDisplayName("Common.CreatedOn")]
        public virtual DateTime? CreatedOnUtc { get; set; }

        [WfmResourceDisplayName("Admin.Common.Fields.UpdatedOn")]
        public virtual DateTime? UpdatedOnUtc { get; set; }

        [WfmResourceDisplayName("Common.CreatedOn")]
        public virtual DateTime? CreatedOn
        {
            get
            {
                DateTime dt = DateTime.MinValue.AddDays(1);     // +1 to avoid possbile validation error for timezones < UTC
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

        [WfmResourceDisplayName("Admin.Common.Fields.UpdatedOn")]
        public virtual DateTime? UpdatedOn
        {
            get
            {
                DateTime dt = DateTime.MinValue.AddDays(1);     // +1 to avoid possbile validation error for timezones < UTC
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
