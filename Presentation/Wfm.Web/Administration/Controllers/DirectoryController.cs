using System;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Services.DirectoryLocation;
using Wfm.Admin.Models.Directory;

namespace Wfm.Admin.Controllers
{
    public class DirectoryController : BaseAdminController
    {
        #region Fields

        private readonly IMapLookupService _mapLookupService;

        #endregion

        #region Ctor
        public DirectoryController(
            IMapLookupService mapLookupService
        )
        {
            _mapLookupService = mapLookupService;
        }

        #endregion

        // GET: /Direction/MapLookup

        public ActionResult MapLookup(string addressLine1, string cityName, string stateProvinceName, string countryName)
        {
            var model = new MapModel()
            {
                MapUrl = _mapLookupService.LookupMap(addressLine1, cityName, stateProvinceName, countryName)
            };

            return PartialView(model);
        }
    }
}

