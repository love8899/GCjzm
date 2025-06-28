using System.Text.RegularExpressions;

namespace Wfm.Web.Framework.Seo
{
    public static class SeoHelper
    {
        public static string ToSeoUrl( string url)
        {
            // make the url lowercase
            string encodedUrl = (url ?? "").ToLower();

            // replace & with -
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "-");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }


        public static string ToSeoUrl(string url, string keyWord)
        {
            string seoUrl = ToSeoUrl(url);

            if (seoUrl.Contains(keyWord))
                return seoUrl;
            else
                return string.Format("{0}-{1}", seoUrl, keyWord);
        }

        public static string ToSeoFriendlyString(string url)
        {
            string encodedUrl = url;

            // replace & with space
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", " ");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-zA-Z0-9]", " ");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @" +", " ");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim();

            return encodedUrl;
        }

    }
}
