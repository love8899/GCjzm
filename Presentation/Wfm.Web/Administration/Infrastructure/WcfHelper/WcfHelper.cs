using System.Configuration;

namespace Wfm.Admin.Infrastructure.WcfHelper
{
    public static class WcfHelper
    {
        private static void GetWcfUserCredentials(out string userName, out string password)
        {
            var appSettings = ConfigurationManager.AppSettings;
            userName = appSettings["WebServiceuserName"];
            password = appSettings["WebServicePassword"];
        }

        public static  ClientServiceReference.WfmServiceClient GetClientService(out string userName, out string password)
        {
            GetWcfUserCredentials(out userName, out password);
            return new ClientServiceReference.WfmServiceClient("BasicHttpBinding_IWfmService");
        }
            
    }
}