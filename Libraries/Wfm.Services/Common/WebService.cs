using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Accounts;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Logging;


namespace Wfm.Services.Common
{
    public class WebService:IWebService
    {
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        private readonly IFranciseAddressService _franchiseAddressService;
        private readonly ILogger _logger;
        private readonly IJobBoardService _jobBoardsService;
        private readonly IJobOrderService _jobOrderService;

        public WebService(ICompanyDivisionService companyDivisionService, IAccountService accountService,
                    IWorkContext workContext, IFranciseAddressService franchiseAddressService, ILogger logger,
                    IJobBoardService jobBoardsService, IJobOrderService jobOrderService)
        {
            _companyDivisionService = companyDivisionService;
            _accountService = accountService;
            _workContext = workContext;
            _franchiseAddressService = franchiseAddressService;
            _logger = logger;
            _jobBoardsService = jobBoardsService;
            _jobOrderService = jobOrderService;
        }

        public string PublishJobOrder(JobOrder jo, string jobBoards)
        {
            string[] jobBoardIds = jobBoards.Split(new Char[] { ',' });
            StringBuilder errors = new StringBuilder();

            foreach (string boardId in jobBoardIds)
            {
                int id = 0;
                if (Int32.TryParse(boardId, out id))
                {
                    var jobBoard = _jobBoardsService.Retrieve(id);
                    string error = string.Empty;
                    switch (jobBoard.JobBoardName)
                    {
                        case "Monster":
                            {
                                CallMonsterWebService(jobBoard.JobBoardUrl, jo, jobBoard.UserName, jobBoard.Password,jobBoard.BoardId ,out error);
                                if (!String.IsNullOrWhiteSpace(error))
                                {
                                    errors.AppendLine("Fail to publish the job order to Monster:");
                                    errors.AppendLine(error);
                                }                               
                                break;
                            }
                        case "Indeed": errors.AppendLine("Fail to publish the job order to Indeed!"); break;
                        default: errors.AppendLine(String.Concat("Fail to publish the job order to ",jobBoard.JobBoardName,"!")); break;
                    }
                }              

            }
            return errors.ToString();
        }

        public void CallMonsterWebService(string url, JobOrder jo,string userName, string password,int boardId,out string error)
        {
            error= string.Empty;
            
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(jo,userName,password,boardId, out error);

            if (String.IsNullOrWhiteSpace(error)&& ValidateJobXML(soapEnvelopeXml,out error))
            {
                HttpWebRequest webRequest = CreateWebRequest(url);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                string soapResult;
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XDocument doc = XDocument.Parse(soapResult);
                        var elements = doc.Elements().Descendants();
                        var descriptions = String.Join(";", elements.Where(x => x.Name.LocalName == "Description").Select(x => x.Value));
                        XAttribute postingAttr = elements.Attributes("postingId").FirstOrDefault();
                        if (postingAttr == null)
                        {
                            //this error is from Monster Server
                            error = descriptions;
                            _logger.Error(error);
                        }
                        else
                        {
                            jo.MonsterPostingId = postingAttr.Value;
                            //_jobOrderService.UpdateJobOrder(jo);
                        }
                    }
                }
            }
        }

        public HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;

        }

        public XmlDocument CreateSoapEnvelope(JobOrder jobOrder,string userName, string password,int boardId,out string errors)
        {
            errors = string.Empty;
            StringBuilder error_message = new StringBuilder();
            string monsterUserName = userName;//"xrtpjobsx01";//use Test user name xrtpjobsx01 
            //string password = "rtp987654";
            XmlDocument soapEnvelop = new XmlDocument();
            StringBuilder xml = new StringBuilder();

            xml.AppendLine("<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'>");
            xml.AppendLine("<SOAP-ENV:Header>");
            xml.AppendLine("<mh:MonsterHeader xmlns:mh='http://schemas.monster.com/MonsterHeader'>");
            xml.AppendLine("<mh:MessageData>");
            xml.AppendLine(String.Concat("<mh:MessageId>Company Jobs created on ", DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss tt"), "</mh:MessageId>"));
            xml.AppendLine(String.Concat("<mh:Timestamp>", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), "</mh:Timestamp>"));
            xml.AppendLine("</mh:MessageData>");
            xml.AppendLine("</mh:MonsterHeader>");
            xml.AppendLine("<wsse:Security xmlns:wsse='http://schemas.xmlsoap.org/ws/2002/04/secext'>");
            xml.AppendLine("<wsse:UsernameToken>");
            xml.AppendLine(String.Concat("<wsse:Username>", monsterUserName, "</wsse:Username>"));
            xml.AppendLine(String.Concat("<wsse:Password>", password, "</wsse:Password>"));
            xml.AppendLine("</wsse:UsernameToken>");
            xml.AppendLine("</wsse:Security>");
            xml.AppendLine("</SOAP-ENV:Header>");
            xml.AppendLine("<SOAP-ENV:Body>");
            //RefCode <=50 required
            //InventoryType : Transactional Slotted AreaWideTransactional AreaWideSlotted
            xml.AppendLine(String.Concat("<Job jobRefCode='"
                                        , jobOrder.Id
                                        , @"' jobAction='addOrUpdate' inventoryType='transactional' jobComplete='true'
                                xmlns='http://schemas.monster.com/Monster' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
                                xsi:schemaLocation='http://schemas.monster.com/Monster     http://schemas.monster.com/Current/xsd/Monster.xsd'>"));
            xml.AppendLine("<RecruiterReference>");
            xml.AppendLine(String.Concat("<UserName>", monsterUserName, "</UserName>"));
            xml.AppendLine("</RecruiterReference>");
            /*
             * http://integrations.monster.com/Toolkit/Enumeration/CoreMonsterChannels. Select Core Monster Channels to find the country and language ChannelID. 
             * ChannelId	Country	Language	ChannelAlias
                    2681	Czech Republic	Czech	CZCZ
                    1426	Denmark	Danish	DKDK
                    128	Netherlands	Dutch	NLDU
                    257	Belgium	Dutch	BEDU
                    13	Canada	English	CAEN
                    58	US	English	MONS
                    200	UK	English	UKEN
                    209	Belgium	English	BEEN
                    431	Ireland	English	IEEN
                    481	Czech Republic	English	CZEN
                    615	Luxembourg	English	LUEN
                    1430	Finland	Finnish	FIFN
                    14	Canada	French	CAFR
                    120	France	French	FRFR
                    256	Belgium	French	BEFR
                    621	Luxembourg	French	LUFR
                    916	Switzerland	French	SWFR
                    419	Germany	German	DEGE
                    620	Luxembourg	German	LUDE
                    800	Switzerland	German	SWGE
                    3252	Austria	German	ATGE
                    3710	Hungary	Hungarian	HUHN
                    442	Italy	Italian	ITIT
                    1425	Norway	Norwegian	NONO
                    3709	Poland	Polish	PLPO
                    5324	Slovakia	Slovak	SKSK
                    437	Spain	Spanish	SPES
                    4171	Mexico	Spanish	MOSPMX
                    1631	Sweden	Swedish	JLSW

             */
            xml.AppendLine("<Channel monsterId='13'/>");//Optional;When added, the channel tag lets the system know which language to use on the Twitter Card.
            xml.AppendLine("<JobInformation>");
            //JobTitle is required, <=100
            if (jobOrder.JobTitle.Length > 100)
            {
                error_message.AppendLine("Job Title is too long (max 100)!");
                _logger.Warning("Job Title is too long!");
                xml.AppendLine(String.Concat("<JobTitle>", jobOrder.JobTitle.Substring(0, 100), "</JobTitle>"));
            }
            else
                xml.AppendLine(String.Concat("<JobTitle>", jobOrder.JobTitle, "</JobTitle>"));
            //JobLevel is optional
            /*
             * LevelId	Description	
                16	Student (High School)	 
                10	Student (Undergraduate/Graduate)	 
                11	Entry Level	 
                12	Experienced (Non-Manager)	 
                13	Manager (Manager/Supervisor of Staff)	 
                14	Executive (SVP, VP, Department Head, etc)	 
                15	Senior Executive (President, CFO, etc)
             */
            xml.AppendLine("<JobLevel monsterId='12'/>");
            /*JobType is optional 
             * 
                MonsterId	Alias	
                1	Employee	 
                2	Temporary / Contract	 
                3	Intern	 
                20	Seasonal	 
                75	Volunteer
             */
            if (jobOrder.JobOrderType.JobOrderTypeName == "Permanent")
                xml.AppendLine(String.Concat("<JobType monsterId='1'/>"));//need to map our job order type to Monster Job Type
            if (jobOrder.JobOrderType.JobOrderTypeName == "Temporary" || jobOrder.JobOrderType.JobOrderTypeName == "Contract")
                xml.AppendLine(String.Concat("<JobType monsterId='2'/>"));//need to map our job order type to Monster Job Type
            if (jobOrder.JobOrderType.JobOrderTypeName == "Internship")
                xml.AppendLine(String.Concat("<JobType monsterId='3'/>"));//need to map our job order type to Monster Job Type
            if (jobOrder.JobOrderType.JobOrderTypeName == "Seasonal")
                xml.AppendLine(String.Concat("<JobType monsterId='20'/>"));//need to map our job order type to Monster Job Type


            /*
             * required
                MonsterId	Alias	
                4	Full Time	 
                5	Part Time	 
                26	Per Diem
             */
            if (jobOrder.JobOrderType.JobOrderTypeName == "Part-time")
                xml.AppendLine("<JobStatus monsterId='5'/>");

            else if (jobOrder.JobOrderType.JobOrderTypeName == "Full time")
                xml.AppendLine("<JobStatus monsterId='4'/>");
            else
                xml.AppendLine("<JobStatus monsterId='26'/>");
            if (jobOrder.SalaryMax.HasValue || jobOrder.SalaryMin.HasValue)
            {
                xml.AppendLine("<Salary>");
                //optional
                /*
                 * 
                    MonsterId	Currency	Description	
                    1	USD	US Dollars	 
                    2	EUR	Euro	 
                    3	ARS	Argentina Peso	 
                    4	AUD	Australia Dollar	 
                    5	BEF	Belgium Franc	 
                    6	BRL	Brazil Real	 
                    7	CAD	Canada Dollar	 
                    8	CHF	Switzerland Franc	 
                    9	CNY	China Yuan Renmimbi	 
                    10	CZK	Czech Republic Koruna	 
                    11	DEM	Germany Deutsche Mark	 
                    12	ESP	Spain Peseta	 
                    13	FJD	Fiji Dollar	 
                    14	FRF	France Franc	 
                    15	GBP	United Kingdom Pound	 
                    16	GRD	Greece Drachma	 
                    17	HKD	Hong Kong Dollar	 
                    18	HUF	Hungary Forint	 
                    19	IDR	Indonesia Rupiah	 
                    20	IEP	Ireland Punt	 
                    21	ILS	Isreal New Shekel	 
                    22	INR	India Rupee	 
                    23	ITL	Italy Lira	 
                    24	JPY	Japan Yen	 
                    25	KRW	South Korea Won	 
                    26	MXN	Mexico Peso	 
                    27	MYR	Malaysia Ringgit	 
                    28	NLG	Netherlands Guilder	 
                    29	NZD	New Zealand Dollar	 
                    30	PLN	Poland Zloty	 
                    31	RUR	Russia Ruble	 
                    32	SEK	Sweden Krona	 
                    33	SGD	Singapore Dollar	 
                    34	TWD	Taiwan Dollar	 
                    35	ZAR	South Africa Rand	 
                    36	LUF	Luxemborg Franc	 
                    37	NOK	Norwegian Krone	 
                    38	DKK	Danish Krone	 
                    39	FIM	Finnish Mark	 
                    40	SKK	Slovak Koruna	 
                    41	ROL	Romanian Leu	 
                    42	IQD	Iraqi Dinar	 
                    43	PHP	Philippine Peso	 
                    44	SAR	Saudi Riyal	 
                    45	THB	Thai Baht	 
                    46	AED	United Arab Emirates Dirham	 
                    47	CLP	Chilean Peso	 
                    48	COP	Colombian Peso	 
                    49	GTQ	Guatemalan Quetzal	 
                    50	PEN	Peruvian Neuevo Sol	 
                    51	TND	Tunisian Dinar	 
                    52	TRY	Turkey New Lira	 
                    53	TTD	Trinidad and Tobago Dollar	 
                    54	UYU	Uruguayan New Peso	 
                    55	VEB	Venezuelan Bolivar	 
                    56	PKR	Pakistan Rupee	 
                    57	EGP	Egyptian Pounds	 
                    58	PGK	Papua New Guinea Kina	 
                    59	BOB	Bolivia Bolivianos	 
                    60	PYG	Paraguay, Guarani	 
                    61	MTL	Malta Lira	 
                    62	BGN	Bulgaria Lev	 
                    63	CYP	Cyprus Pound	 
                    64	EEK	Estonia Kroon	 
                    65	ISK	Iceland Krona	 
                    66	LVL	Latvia Lats	 
                    67	LTL	Lithuania Litas	 
                    68	SIT	Slovenia Tolar
                 */
                xml.AppendLine("<Currency monsterId='7'/>");
                if(jobOrder.SalaryMin.HasValue)
                    xml.AppendLine(String.Concat("<SalaryMin>", jobOrder.SalaryMin.Value.ToString(), "</SalaryMin>"));
                if(jobOrder.SalaryMax.HasValue)
                    xml.AppendLine(String.Concat("<SalaryMax>", jobOrder.SalaryMax.Value.ToString(), "</SalaryMax>"));
                /*
                 * 
                    SalaryTypeID	Description	
                    1	Per Year	 
                    2	Per Hour	 
                    3	Per Week	 
                    4	Per Month	 
                    5	Biweekly	 
                    6	Per Day	 
                    7	Quarter	 
                    8	Other	 
                    9	Piece Rate
                 */
                xml.AppendLine("<CompensationType monsterId='5'/>");
                xml.AppendLine("</Salary>");
            }
            xml.AppendLine("<Contact hideAll='false' hideAddress='false' hideStreetAddress='false' hideCity='false' hideState='false' hidePostalCode='false' hideCountry='false' hideContactInfoField='false' hideCompanyName='false' hideEmailAddress='false' hideFax='false' hideName='false' hidePhone='false'>");
            //Contact/Name is optional, <=255
            if (_workContext.CurrentAccount.FullName.Length > 255)
            {
                error_message.AppendLine("Contact Name is too long (max 255)!");
                _logger.Warning("Contact Name is too long!");
                xml.AppendLine(String.Concat("<Name>", _workContext.CurrentAccount.FullName.Substring(0, 255), "</Name>"));
            }
            else
                xml.AppendLine(String.Concat("<Name>", _workContext.CurrentAccount.FullName, "</Name>"));

            //CompanyName optional <=255
            if (jobOrder.Company.CompanyName.Length > 255)
            {
                error_message.AppendLine("Company name is too long (max 255)!");
                _logger.Warning("Company name is too long!");
                xml.AppendLine(String.Concat("<CompanyName>", jobOrder.Company.CompanyName.Substring(0, 255), "</CompanyName>"));
            }
            else
                xml.AppendLine(String.Concat("<CompanyName>", jobOrder.Company.CompanyName, "</CompanyName>"));

            //address is optional
            var franchiseLocation = _franchiseAddressService.GetAllFranchiseAddressByFranchiseGuid(_workContext.CurrentFranchise.FranchiseGuid).FirstOrDefault();
            if (franchiseLocation != null)
            {
                xml.AppendLine("<Address>");
                if(!String.IsNullOrWhiteSpace(franchiseLocation.AddressLine1))
                    xml.AppendLine(String.Concat("<StreetAddress>", franchiseLocation.AddressLine1, "</StreetAddress>"));
                if (!String.IsNullOrWhiteSpace(franchiseLocation.AddressLine2))
                    xml.AppendLine(String.Concat("<StreetAddress2>", franchiseLocation.AddressLine2, "</StreetAddress2>"));
                xml.AppendLine(String.Concat("<City>", franchiseLocation.City.CityName, "</City>"));
                xml.AppendLine(String.Concat("<State>", franchiseLocation.StateProvince.Abbreviation, "</State>"));
                xml.AppendLine(String.Concat("<CountryCode>", franchiseLocation.Country.TwoLetterIsoCode, "</CountryCode>"));
                xml.AppendLine(String.Concat("<PostalCode>", franchiseLocation.PostalCode, "</PostalCode>"));
                xml.AppendLine("</Address>");
            }
            //phone optional Between 8 and 50 characters
            if(_workContext.CurrentAccount.WorkPhone.Length>=8&&_workContext.CurrentAccount.WorkPhone.Length<=50)
                xml.AppendLine(String.Concat("<Phones><Phone phoneType='work'>", _workContext.CurrentAccount.WorkPhone, "</Phone></Phones>"));
            //Email is optional <= 100 characters
            if (_workContext.CurrentAccount.Email.Length <= 100)
                xml.AppendLine(String.Concat("<E-mail>", _workContext.CurrentAccount.Email, "</E-mail>"));
            else
                xml.AppendLine(String.Concat("<E-mail>", _workContext.CurrentAccount.Email.Substring(0, 100), "</E-mail>"));
            xml.AppendLine(String.Concat("<WebSite>", _workContext.CurrentFranchise.WebSite, "</WebSite>"));
            xml.AppendLine("</Contact>");
            var location = _companyDivisionService.GetCompanyLocationById(jobOrder.CompanyLocationId);
            if (location != null)
            {
                xml.AppendLine("<PhysicalAddress>");
                if (!String.IsNullOrWhiteSpace(location.AddressLine1))
                    xml.AppendLine(String.Concat("<StreetAddress>", location.AddressLine1, "</StreetAddress>"));
                if (!String.IsNullOrWhiteSpace(location.AddressLine2))
                    xml.AppendLine(String.Concat("<StreetAddress2>", location.AddressLine2, "</StreetAddress2>"));
                xml.AppendLine(String.Concat("<City>", location.City.CityName, "</City>"));
                xml.AppendLine(String.Concat("<State>", location.StateProvince.Abbreviation, "</State>"));
                xml.AppendLine(String.Concat("<CountryCode>", location.Country.TwoLetterIsoCode, "</CountryCode>"));
                xml.AppendLine(String.Concat("<PostalCode>", location.PostalCode, "</PostalCode>"));
                xml.AppendLine("</PhysicalAddress>");
            }
            xml.AppendLine("<DisableApplyOnline>false</DisableApplyOnline>");
            xml.AppendLine("<HideCompanyInfo>false</HideCompanyInfo>");
            //JobBody >= 25 characters required
            if (jobOrder.JobDescription.Length >= 25)
            {
                string noHTML = Regex.Replace(jobOrder.JobDescription, @"<[^>]+>|&nbsp;", "").Trim();
                string description = Regex.Replace(noHTML, @"\s{2,}", " ");
                xml.AppendLine(String.Concat("<JobBody>", description, "</JobBody>"));
            }
            else
            {
                error_message.AppendLine("JobBody must be equal to or greater than 25!");
                _logger.Error("JobBody must be equal to or greater than 25!");
            }
            //xml.AppendLine("<AdditionalSearchKeywords></AdditionalSearchKeywords>");//optional
            //Education level is optional
            /*
             * 
                EducationLevelID	Description	
                -1	Unspecified	 
                1	High School or equivalent	 
                2	Certification	 
                3	Vocational	 
                4	Associate Degree	 
                5	Bachelor's Degree	 
                6	Master's Degree	 
                7	Doctorate	 
                8	Professional	 
                9	Some College Coursework Completed	 
                10	Vocational - HS Diploma	 
                11	Vocational - Degree	 
                12	Some High School Coursework
             */
            xml.AppendLine("<EducationLevel monsterId='-1'/>");
            xml.AppendLine("</JobInformation>");
            xml.AppendLine("<JobPostings>");
            xml.AppendLine("<JobPosting desiredDuration='60' bold='true'>");
            //location is required
            xml.AppendLine("<Location>");
            xml.AppendLine(String.Concat("<City>", location.City.CityName, "</City>"));
            xml.AppendLine(String.Concat("<State>", location.StateProvince.Abbreviation, "</State>"));
            xml.AppendLine(String.Concat("<CountryCode>", location.Country.TwoLetterIsoCode, "</CountryCode>"));
            xml.AppendLine(String.Concat("<PostalCode>", location.PostalCode, "</PostalCode>"));
            xml.AppendLine("</Location>");
            /*
             * JobCategory is required (need to map our category with Monster category)
             * default set to 11 (other)
             */
            xml.AppendLine(String.Concat("<JobCategory monsterId='11'/>"));
            xml.AppendLine("<JobOccupations>");
            /*
             * JobOccupation is required
             * default is Other 11892
             */
            xml.AppendLine("<JobOccupation monsterId='11892'/>");
            xml.AppendLine("</JobOccupations>");
            /*
             * Job Board Name	Test Board Id	Production Board Id
                    Monster	178	1
                    Diversity	7007	3902
                    CAN Direct	7008	6955
                    Veterans	7009	6965
                    Monster Social Ads	-	7082
             */
            xml.AppendLine(String.Concat("<BoardName monsterId='",boardId,"'/>"));
            xml.AppendLine("<DisplayTemplate monsterId='1'/>");//optional
            xml.AppendLine("<Industries>");//optional
            xml.AppendLine("<Industry>");
            xml.AppendLine("<IndustryName monsterId='85'/>");//max 3 industries, default other 85
            xml.AppendLine("</Industry>");
            xml.AppendLine("</Industries>");
            xml.AppendLine("</JobPosting>");
            xml.AppendLine("</JobPostings>");
            xml.AppendLine("</Job>");
            xml.AppendLine("</SOAP-ENV:Body>");
            xml.AppendLine("</SOAP-ENV:Envelope>");
            soapEnvelop.LoadXml(xml.ToString());
            errors = error_message.ToString();
            return soapEnvelop;

        }

        public void InsertSoapEnvelopeIntoWebRequest(System.Xml.XmlDocument soapEnvelopeXml, System.Net.HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

        }

        public bool ValidateJobXML(XmlDocument document, out string errorMessage)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("http://schemas.monster.com/Monster", "http://schemas.monster.com/Current/XSD/Job.xsd");
            XDocument xml = XDocument.Load(new XmlNodeReader(document)); ;
            bool errors = false;
            errorMessage = string.Empty;
            StringBuilder error_message = new StringBuilder();
            xml.Validate(schemas, (o, s) =>
            {
                error_message.AppendLine(s.Message);
                errors = true;
            });
            if (!errors)
                return true;
            else
            {
                errorMessage = error_message.ToString();
                _logger.Error(error_message.ToString());
                return false;
            }
        }
    }
}
