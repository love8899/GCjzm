using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System;
using Wfm.Core.Domain;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.DirectoryLocation;
using Wfm.Web.Models.Common;
using Wfm.Web.Framework.Themes;


namespace Wfm.Web.Controllers
{
    public partial class CommonController : BasePublicController
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ICandidateService _candidateService;
        private readonly ISourceService _sourceService;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;

        private readonly CommonSettings _commonSettings;
        private readonly FranchiseInformationSettings _publicWebInformationSettings;

        #endregion


        #region Constructors

        public CommonController(
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ICandidateService candidateService,
            ISourceService sourceService,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            CommonSettings commonSettings,
            FranchiseInformationSettings publicWebInformationSettings
            )
        {
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._cityService = cityService;
            this._candidateService = candidateService;
            this._sourceService = sourceService;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;

            this._commonSettings = commonSettings;
            this._publicWebInformationSettings = publicWebInformationSettings;
        }

        #endregion


        #region Utilities

        // access denied
        public ActionResult AccessDenied()
        {
            this.Response.StatusCode = 403;
            this.Response.TrySkipIisCustomErrors = true;

            return View();
        }


        // page not found
        public ActionResult PageNotFound()
        {
            this.Response.StatusCode = 404;
            this.Response.TrySkipIisCustomErrors = true;

            return View();
        }


        // unkown error
        public ActionResult UnknownError()
        {
            this.Response.StatusCode = 500;
            this.Response.TrySkipIisCustomErrors = true;

            return View();
        }


        // yes or no
        public ActionResult YesOrNo(string text)
        {
            ViewBag.YesOrNoText = text;
            return View();
        }

        #endregion


        #region Methods

        public ActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }


        //publicWeb is closed
        public ActionResult SystemClosed()
        {
            return View();
        }


        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }


        //header links
        [ChildActionOnly]
        public ActionResult HeaderLinks()
        {
            var model = new HeaderLinksModel {
                IsAuthenticated = false,
                CandidateEmailUsername = string.Empty
            };
            
            if (Request.IsAuthenticated)
            {
                Candidate candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
                if (candidate != null)
                {
                    model.IsAuthenticated = true;
                    model.CandidateEmailUsername = candidate.Email;
                }
            }
 
            return PartialView(model);
        }


        public ActionResult RobotsTextFile()
        {
            var disallowPaths = new List<string>()
                                    {
                                        "/attachment",
                                        "/app_data",
                                        "/bin/",
                                        "/content/",
                                        "/scripts",
                                        "/themes",
                                    };
            var localizableDisallowPaths = new List<string>()
                                               {
                                                   "/admin/",
                                                   "/job-seekers/login",
                                                   "/job-seekers/register",
                                                   "/job-seekers/retrieve-password",
                                               };


            const string newLine = "\r\n"; //Environment.NewLine
            var sb = new StringBuilder();
            sb.Append("User-agent: *");
            sb.Append(newLine);
            //usual paths
            foreach (var path in disallowPaths)
            {
                sb.AppendFormat("Disallow: {0}", path);
                sb.Append(newLine);
            }
            //localizable paths (without SEO code)
            foreach (var path in localizableDisallowPaths)
            {
                sb.AppendFormat("Disallow: {0}", path);
                sb.Append(newLine);
            }

            //if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            //{
            //    //URLs are localizable. Append SEO code
            //    foreach (var language in _languageService.GetAllLanguages(franchiseId: _publicWebContext.CurrentPublicWeb.Id))
            //    {
            //        foreach (var path in localizableDisallowPaths)
            //        {
            //            sb.AppendFormat("Disallow: {0}{1}", language.UniqueSeoCode, path);
            //            sb.Append(newLine);
            //        }
            //    }
            //}

            Response.ContentType = "text/plain";
            Response.Write(sb.ToString());
            return null;
        }

        #endregion


        #region // JsonResult: GetCascadeCountries

        public JsonResult GetCascadeCountries()
        {
            var countries = _countryService.GetAllCountries();
            var countryDropDownList = new List<SelectListItem>();
            foreach (var c in countries)
            {
                var item = new SelectListItem()
                {
                    Text = c.CountryName,
                    Value = c.Id.ToString()
                };
                countryDropDownList.Add(item);
            }

            return Json(countryDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeStateProvinces

        public JsonResult GetCascadeStateProvinces(string countryId)
        {
            var stateProvinceDropDownList = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(countryId))
            {
                var stateProvinces = _stateProvinceService.GetAllStateProvincesByCountryId(Convert.ToInt32(countryId));

                foreach (var p in stateProvinces)
                {
                    var item = new SelectListItem()
                    {
                        Text = p.StateProvinceName,
                        Value = p.Id.ToString()
                    };
                    stateProvinceDropDownList.Add(item);
                }
            }
            return Json(stateProvinceDropDownList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region // JsonResult: GetCascadeCities

        public JsonResult GetCascadeCities(string stateProvinceId)
        {
            var cityDropDownList = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(stateProvinceId))
            {
                var citites = _cityService.GetAllCitiesByStateProvinceId(Convert.ToInt32(stateProvinceId));

                foreach (var p in citites)
                {
                    var item = new SelectListItem()
                    {
                        Text = p.CityName,
                        Value = p.Id.ToString()
                    };
                    cityDropDownList.Add(item);
                }
            }
            return Json(cityDropDownList, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region GetAllSources

        public JsonResult GetAllSources()
        {
            var result = _sourceService.GetAllSources().OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id)
                .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.SourceName });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
