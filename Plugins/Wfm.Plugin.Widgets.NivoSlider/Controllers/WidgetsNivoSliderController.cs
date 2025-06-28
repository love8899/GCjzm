using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Plugin.Widgets.NivoSlider.Infrastructure.Cache;
using Wfm.Plugin.Widgets.NivoSlider.Models;
using Wfm.Services.Configuration;
using Wfm.Services.Localization;
using Wfm.Services.Media;
using Wfm.Services.Franchises;
using Wfm.Web.Framework.Controllers;

namespace Wfm.Plugin.Widgets.NivoSlider.Controllers
{
    public class WidgetsNivoSliderController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IFranchiseContext _franchiseContext;
        private readonly IFranchiseService _franchiseService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;

        public WidgetsNivoSliderController(IWorkContext workContext,
            IFranchiseContext franchiseContext,
            IFranchiseService franchiseService, 
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._franchiseContext = franchiseContext;
            this._franchiseService = franchiseService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
        }

        protected string GetPictureUrl(int pictureId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, pictureId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false);
                //little hack here. nulls aren't cacheable so set it to ""
                if (url == null)
                    url = "";

                return url;
            });
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen franchise scope
            var franchiseScope = this.GetActiveFranchiseScopeConfiguration(_franchiseService, _workContext);
            var nivoSliderSettings = _settingService.LoadSetting<NivoSliderSettings>(franchiseScope);
            var model = new ConfigurationModel();
            model.Picture1Id = nivoSliderSettings.Picture1Id;
            model.Text1 = nivoSliderSettings.Text1;
            model.Link1 = nivoSliderSettings.Link1;
            model.Picture2Id = nivoSliderSettings.Picture2Id;
            model.Text2 = nivoSliderSettings.Text2;
            model.Link2 = nivoSliderSettings.Link2;
            model.Picture3Id = nivoSliderSettings.Picture3Id;
            model.Text3 = nivoSliderSettings.Text3;
            model.Link3 = nivoSliderSettings.Link3;
            model.Picture4Id = nivoSliderSettings.Picture4Id;
            model.Text4 = nivoSliderSettings.Text4;
            model.Link4 = nivoSliderSettings.Link4;
            model.Picture5Id = nivoSliderSettings.Picture5Id;
            model.Text5 = nivoSliderSettings.Text5;
            model.Link5 = nivoSliderSettings.Link5;
            model.ActiveFranchiseScopeConfiguration = franchiseScope;
            if (franchiseScope > 0)
            {
                model.Picture1Id_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Picture1Id, franchiseScope);
                model.Text1_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Text1, franchiseScope);
                model.Link1_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Link1, franchiseScope);
                model.Picture2Id_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Picture2Id, franchiseScope);
                model.Text2_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Text2, franchiseScope);
                model.Link2_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Link2, franchiseScope);
                model.Picture3Id_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Picture3Id, franchiseScope);
                model.Text3_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Text3, franchiseScope);
                model.Link3_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Link3, franchiseScope);
                model.Picture4Id_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Picture4Id, franchiseScope);
                model.Text4_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Text4, franchiseScope);
                model.Link4_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Link4, franchiseScope);
                model.Picture5Id_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Picture5Id, franchiseScope);
                model.Text5_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Text5, franchiseScope);
                model.Link5_OverrideForFranchise = _settingService.SettingExists(nivoSliderSettings, x => x.Link5, franchiseScope);
            }

            return View("~/Plugins/Widgets.NivoSlider/Views/WidgetsNivoSlider/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen franchise scope
            var franchiseScope = this.GetActiveFranchiseScopeConfiguration(_franchiseService, _workContext);
            var nivoSliderSettings = _settingService.LoadSetting<NivoSliderSettings>(franchiseScope);
            nivoSliderSettings.Picture1Id = model.Picture1Id;
            nivoSliderSettings.Text1 = model.Text1;
            nivoSliderSettings.Link1 = model.Link1;
            nivoSliderSettings.Picture2Id = model.Picture2Id;
            nivoSliderSettings.Text2 = model.Text2;
            nivoSliderSettings.Link2 = model.Link2;
            nivoSliderSettings.Picture3Id = model.Picture3Id;
            nivoSliderSettings.Text3 = model.Text3;
            nivoSliderSettings.Link3 = model.Link3;
            nivoSliderSettings.Picture4Id = model.Picture4Id;
            nivoSliderSettings.Text4 = model.Text4;
            nivoSliderSettings.Link4 = model.Link4;
            nivoSliderSettings.Picture5Id = model.Picture5Id;
            nivoSliderSettings.Text5 = model.Text5;
            nivoSliderSettings.Link5 = model.Link5;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.Picture1Id_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Picture1Id, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Picture1Id, franchiseScope);
            
            if (model.Text1_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Text1, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Text1, franchiseScope);
            
            if (model.Link1_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Link1, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Link1, franchiseScope);
            
            if (model.Picture2Id_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Picture2Id, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Picture2Id, franchiseScope);
            
            if (model.Text2_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Text2, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Text2, franchiseScope);
            
            if (model.Link2_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Link2, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Link2, franchiseScope);
            
            if (model.Picture3Id_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Picture3Id, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Picture3Id, franchiseScope);
            
            if (model.Text3_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Text3, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Text3, franchiseScope);
            
            if (model.Link3_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Link3, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Link3, franchiseScope);
            
            if (model.Picture4Id_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Picture4Id, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Picture4Id, franchiseScope);
            
            if (model.Text4_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Text4, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Text4, franchiseScope);

            if (model.Link4_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Link4, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Link4, franchiseScope);

            if (model.Picture5Id_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Picture5Id, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Picture5Id, franchiseScope);

            if (model.Text5_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Text5, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Text5, franchiseScope);

            if (model.Link5_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(nivoSliderSettings, x => x.Link5, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(nivoSliderSettings, x => x.Link5, franchiseScope);
            
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            var nivoSliderSettings = _settingService.LoadSetting<NivoSliderSettings>(_franchiseContext.CurrentFranchise.Id);

            var model = new PublicInfoModel();
            model.Picture1Url = GetPictureUrl(nivoSliderSettings.Picture1Id);
            model.Text1 = nivoSliderSettings.Text1;
            model.Link1 = nivoSliderSettings.Link1;

            model.Picture2Url = GetPictureUrl(nivoSliderSettings.Picture2Id);
            model.Text2 = nivoSliderSettings.Text2;
            model.Link2 = nivoSliderSettings.Link2;

            model.Picture3Url = GetPictureUrl(nivoSliderSettings.Picture3Id);
            model.Text3 = nivoSliderSettings.Text3;
            model.Link3 = nivoSliderSettings.Link3;

            model.Picture4Url = GetPictureUrl(nivoSliderSettings.Picture4Id);
            model.Text4 = nivoSliderSettings.Text4;
            model.Link4 = nivoSliderSettings.Link4;

            model.Picture5Url = GetPictureUrl(nivoSliderSettings.Picture5Id);
            model.Text5 = nivoSliderSettings.Text5;
            model.Link5 = nivoSliderSettings.Link5;

            if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
                string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url) &&
                string.IsNullOrEmpty(model.Picture5Url))
                //no pictures uploaded
                return Content("");


            return View("~/Plugins/Widgets.NivoSlider/Views/WidgetsNivoSlider/PublicInfo.cshtml", model);
        }
    }
}