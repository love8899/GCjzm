using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Wfm.Core.Infrastructure;
using Wfm.Services.Configuration;

namespace Wfm.Web.Framework.Security
{
    public class reCaptchaValidate
    {
        public static bool Validate(string mainresponse)
        {
            try
            {
                var _settingService = EngineContext.Current.Resolve<ISettingService>();
                string privatekey = _settingService.GetSettingByKey<string>("Google.reCaptcha.SecretKey", null, 0, true);
                 

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" + privatekey + "&response=" + mainresponse);
                WebResponse response = req.GetResponse();

                using (StreamReader readStream = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = readStream.ReadToEnd();
                    JsonResponseObject jobj = JsonConvert.DeserializeObject<JsonResponseObject>(jsonResponse);
                    return jobj.success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public class JsonResponseObject
        {
            public bool success { get; set; }
            [JsonProperty("error-codes")]
            public List<string> errorcodes { get; set; }
        }
    }
}
