using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Common
{
    public partial class MaintenanceModel : BaseWfmModel
    {
        public MaintenanceModel()
        {
            DeleteGuests = new DeleteGuestsModel();
            DeleteAbandonedCarts = new DeleteAbandonedCartsModel();
            DeleteExportedFiles = new DeleteExportedFilesModel();
        }

        public DeleteGuestsModel DeleteGuests { get; set; }
        public DeleteAbandonedCartsModel DeleteAbandonedCarts { get; set; }
        public DeleteExportedFilesModel DeleteExportedFiles { get; set; }

        #region Nested classes

        public partial class DeleteGuestsModel : BaseWfmModel
        {
            [WfmResourceDisplayName("Common.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [WfmResourceDisplayName("Common.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            [WfmResourceDisplayName("Admin.System.Maintenance.DeleteGuests.OnlyWithoutShoppingCart")]
            public bool OnlyWithoutShoppingCart { get; set; }

            public int? NumberOfDeletedCustomers { get; set; }
        }

        public partial class DeleteAbandonedCartsModel : BaseWfmModel
        {
            [WfmResourceDisplayName("Admin.System.Maintenance.DeleteAbandonedCarts.OlderThan")]
            [UIHint("Date")]
            public DateTime OlderThan { get; set; }

            public int? NumberOfDeletedItems { get; set; }
        }

        public partial class DeleteExportedFilesModel : BaseWfmModel
        {
            [WfmResourceDisplayName("Common.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [WfmResourceDisplayName("Common.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedFiles { get; set; }
        }

        #endregion
    }
}
