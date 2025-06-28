using FluentValidation.Attributes;
using Wfm.Admin.Validators.Settings;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Settings
{
    [Validator(typeof(SettingValidator))]
    public partial class SettingModel : BaseWfmEntityModel
    {
        public SettingModel() { }

        public SettingModel(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        [WfmResourceDisplayName("Common.Name")]
        public virtual string Name { get; set; }

        [WfmResourceDisplayName("Common.Value")]
        public virtual string Value { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Franchise")]
        public int FranchiseId { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}