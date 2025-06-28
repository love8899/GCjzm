using System;
using System.Collections;
using System.Linq;
using Wfm.Web.Framework.HtmlHelpers;

namespace Wfm.Web.Framework.Seo.Keyword
{
    public class HtmlToKeywords
    {
        public HtmlToKeywords () {}

        public string Parse (string htmlContent){

            HtmlToText htmlToText = new HtmlToText();
            string text = htmlToText.Convert(htmlContent);

            KeywordAnalyzer ka = new KeywordAnalyzer();
            var k = ka.Analyze(text);

            var keys = (from n in k.Keywords select n).Take(10);

            ArrayList alKeys = new ArrayList();
            foreach (var key in keys)
            {
                alKeys.Add(key.Word);
            }

            return String.Join(",", alKeys.ToArray());
        }


        public string Parse(string htmlContent, int number)
        {

            HtmlToText htmlToText = new HtmlToText();
            string text = htmlToText.Convert(htmlContent);

            KeywordAnalyzer ka = new KeywordAnalyzer();
            var k = ka.Analyze(text);

            var keys = (from n in k.Keywords select n).Take(number);

            ArrayList alKeys = new ArrayList();
            foreach (var key in keys)
            {
                alKeys.Add(key.Word);
            }

            return String.Join(",", alKeys.ToArray());
        }

    }
}
