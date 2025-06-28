using System;

namespace Wfm.Core.Data
{
    public partial class DataSettingsHelper
    {
        private static bool? _databaseIsInstalled;
        public static bool DatabaseIsInstalled(string filePath = null)
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var manager = new DataSettingsManager();
                var settings = manager.LoadSettings(filePath);
                _databaseIsInstalled = settings != null && !String.IsNullOrEmpty(settings.DataConnectionString);
            }
            return _databaseIsInstalled.Value;
        }

        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }
    }
}
