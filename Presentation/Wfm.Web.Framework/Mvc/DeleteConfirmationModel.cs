﻿namespace Wfm.Web.Framework.Mvc
{
    public class DeleteConfirmationModel : BaseWfmEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
    }
}