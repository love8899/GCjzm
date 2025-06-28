using System;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Common;


namespace Wfm.Web.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly FranchiseInformationSettings _publicWebInformationSettings;
        private readonly IThemeProvider _themeProvider;

        private bool _themeIsCached;
        private string _cachedThemeName;


        public ThemeContext(IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            FranchiseInformationSettings publicWebInformationSettings, 
            IThemeProvider themeProvider)
        {
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._publicWebInformationSettings = publicWebInformationSettings;
            this._themeProvider = themeProvider;
        }


        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        public string WorkingThemeName
        {
            get
            {
                if (_themeIsCached)
                    return _cachedThemeName;

                string theme = "";
                if (_publicWebInformationSettings.AllowUserToSelectTheme)
                {
                    if (_workContext.CurrentAccount != null)
                        theme = _workContext.CurrentAccount.GetAttribute<string>(SystemAccountAttributeNames.WorkingThemeName, _genericAttributeService, _workContext.CurrentFranchise.Id);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _publicWebInformationSettings.DefaultWebTheme;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                {
                    var themeInstance = _themeProvider.GetThemeConfigurations()
                        .FirstOrDefault();
                    if (themeInstance == null)
                        throw new Exception("No theme could be loaded");
                    theme = themeInstance.ThemeName;
                }

                //cache theme
                this._cachedThemeName = theme;
                this._themeIsCached = true;
                return theme;
            }
            set
            {
                if (!_publicWebInformationSettings.AllowUserToSelectTheme)
                    return;

                if (_workContext.CurrentFranchise == null)
                    return;

                _genericAttributeService.SaveAttribute(_workContext.CurrentFranchise, SystemAccountAttributeNames.WorkingThemeName, value, _workContext.CurrentFranchise.Id);

                //clear cache
                this._themeIsCached = false;
            }
        }
    }
}
