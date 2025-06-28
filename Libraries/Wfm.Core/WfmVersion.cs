
namespace Wfm.Core
{
    public static class WfmVersion
    {
        /// <summary>
        /// Gets or sets the franchise version
        /// </summary>
        public static string CurrentVersion 
        {
            get
            {
                //return "2.1.1";
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
