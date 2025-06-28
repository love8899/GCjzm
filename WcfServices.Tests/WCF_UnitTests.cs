using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WcfServices.Tests.ServiceReference1;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WcfServices.Tests
{
    
    [TestClass]
    public class WCF_UnitTests
    {
        const string WebServiceuserName ="WfmWcfUser";
        const string WebServicePassword = "9C50A88469A4D72921682438A914F7";
        const string ServiceURL = "https://gc-employment.com/wcf/WfmService.svc/json/"; //"https://wfm.eastus.cloudapp.azure.com/qawcf/WfmService.svc/json/"; //"http://localhost:3630/WfmService.svc/json/"; //  

        private string CreateSession()
        {
            string _sessionId = null;

            var client = new HttpClient();
            var uri = new Uri(ServiceURL + "AuthenticateUser/?p1=TestUser&p2=TestUser123!");

            var stringContent = new StringContent(String.Empty);
            var response = client.PostAsync(uri, stringContent).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var info = JsonConvert.DeserializeObject<AuthenticationResult>(content);
                _sessionId = info.SessionId;
            }

            return _sessionId;
        }

        [TestMethod]
        public void WCF_TestPingWfmService()
        {
            // assemble
            ServiceReference1.WfmServiceClient svcClient = new ServiceReference1.WfmServiceClient("BasicHttpBinding_IWfmService");

            // act
            svcClient.PingWfmService();
            svcClient.Close();

            //assert
        }


        [TestMethod]
        public void WCF_TestLoginService()
        {
            // assemble 
            ServiceReference1.WfmServiceClient svcClient = new ServiceReference1.WfmServiceClient("BasicHttpBinding_IWfmService");

            //act
            var result = svcClient.AuthenticateUser("tom.jones@magna.com", "123");
            svcClient.Close();

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.SessionId);
        }

        [TestMethod]
        public void WCF_TestWorkContexts()
        {
            //assemble
            ServiceReference1.WfmServiceClient svcClient = new ServiceReference1.WfmServiceClient("BasicHttpBinding_IWfmService");

            //act
            var result1 = svcClient.AuthenticateUser("Devinder.dhillon@magna.com", "123");
            var session1 = result1.SessionId;

            var result2 = svcClient.AuthenticateUser("travis.hill@magna.com", "123");
            var session2 = result2.SessionId;

            //todo
            var timesheets1 = svcClient.GetEmployeeTimeSheetHistoryByDate(session1, new DateTime(2015, 12, 1), new DateTime(2015, 12, 31), 20, 0,null, null, null, null, null);
            var timesheets2 = svcClient.GetEmployeeTimeSheetHistoryByDate(session2, new DateTime(2015, 10, 1), new DateTime(2015, 11, 1), 20, 0, null, null, null, null, null);

            svcClient.Close();

            // assert
            Assert.Equals(timesheets1.Items.Length, 10);
         
        }

        [TestMethod]
        public void WCF_TestGetDailyTimeSheet() 
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                // act

                var client = new HttpClient();

                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = new Uri(String.Format(ServiceURL + @"GetDailyTimeSheet/?p1={0}&p2={1}&p3={2}&p4={3}&p5={4}&p6={5}&p7={6}&p8={7}&p9={8}",
                                            sessionId, false, 20, 0, "LastName", "asc", "", "", ""));

                client.DefaultRequestHeaders.Host = uri.Host;
                var response = client.GetAsync(uri).GetAwaiter().GetResult();

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    Assert.Fail("Request failed.");
                // assert
                Assert.IsNotNull(content);
            }
            else
                Assert.Fail();
        }

        [TestMethod]
        public void WCF_TestTimeSheetHistoryByDate()
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                 var client = new HttpClient();

                // act

                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = new Uri(String.Format(ServiceURL + @"GetEmployeeTimeSheetHistoryByDate/?p1={0}&p2={1}&p3={2}&p4={3}&p5={4}&p6={5}&p7={6}&p8={7}&p9={8}&p10={9}",
                                            sessionId, new DateTime(2016, 6, 5), new DateTime(2016, 7, 23), 20, 0, "LastName", "desc", "LastName", "like", "77"));
                client.DefaultRequestHeaders.Host = uri.Host;
                var response = client.GetAsync(uri).GetAwaiter().GetResult();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    Assert.Fail("Request failed.");

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else
                Assert.Fail();

        }


        [TestMethod]
        public void WCF_TestGetTimeSheetForApproval()
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                var client = new HttpClient();

                // act


                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = new Uri(String.Format(ServiceURL + @"GetTimeSheetForApproval/?p1={0}&p2={1}&p3={2}&p4={3}&p5={4}&p6={5}&p7={6}&p8={7}&p9={8}&p10={9}",
                                            sessionId,new DateTime(2016, 7, 17), true, 20, 0, "JobStartDateTime", "desc", "", "eq", ""));
                client.DefaultRequestHeaders.Host = uri.Host;
                var response = client.GetAsync(uri).GetAwaiter().GetResult();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    Assert.Fail("Request failed.");

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else
                Assert.Fail();
        }

        [TestMethod]
        public void WCF_TestTimeSheetApproval()
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                var client = new HttpClient();
                
                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = new Uri(ServiceURL + "ApproveTimeSheet");
                client.DefaultRequestHeaders.Host = uri.Host;

                
                // Create the json object for approval
                TimeSheetSlim ts = new TimeSheetSlim()
                {
                    //CandidateId = 86460,
                    CandidateGuid = "C826EE30-8DDD-4A2B-9672-1F55715C2B7F",
                    TimeSheetId = 163630
                };

                var body = new TimeSheetAndSession() { SessionId = sessionId, CandidateTimeSheet = ts };

                // Serialize the object
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(TimeSheetAndSession)); //Create a Json Serializer for our type 

                // use the serializer to write the object to a MemoryStream 
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonSer.WriteObject(ms, body);
                    ms.Position = 0;

                    //use a Stream reader to construct the StringContent (Json) 
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        StringContent newContent = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync(uri, newContent).GetAwaiter().GetResult();

                        if (response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            Assert.IsTrue(true);
                        }
                        else
                            Assert.Fail();
                    }
                }
                
            }
            else
            {
                Assert.Fail();
            }

        }


        public class RootObject
        {
            public string sessionId {get; set;}
            public TimeSheetSlim[] timeSheets { get; set; }
        }

        public class RootObject2
        {
            public string SessionId { get; set; }
            public TimeSheet candidateTimeSheet { get; set; }
        } 
        [TestMethod]
        public void WCF_TestBatchApproval()
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                var client = new HttpClient();

                //act

                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = new Uri(ServiceURL + "ApproveTimeSheets");
                client.DefaultRequestHeaders.Host = uri.Host;

                // Create the json objects for approval
                TimeSheetSlim[] myTimeSheets = new TimeSheetSlim[2];
                myTimeSheets[0] = new TimeSheetSlim()
                {
                    //CandidateId = 1133,
                    CandidateGuid = "92C7DD05-B354-4BEF-9F06-1762784DCF1A",
                    TimeSheetId = 194591
                };

                myTimeSheets[1] = new TimeSheetSlim()
                {
                    //CandidateId = 1133,
                    CandidateGuid = "92C7DD05-B354-4BEF-9F06-1762784DCF1A",
                    TimeSheetId = 194588
                };

                var objectToSerialize = new RootObject();
                objectToSerialize.timeSheets = myTimeSheets;
                objectToSerialize.sessionId = sessionId;

                var ser = JsonConvert.SerializeObject(objectToSerialize);

                // use the serializer to write the object to a MemoryStream 
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(ser)))
                {
                    //use a Stream reader to construct the StringContent (Json) 
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        StringContent newContent = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync(uri, newContent).GetAwaiter().GetResult();

                        if (response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            Assert.IsTrue(true);
                        }
                    }
                }

            }
            else
            {
                Assert.Fail();
            }

        }

        [TestMethod]
        public void WCF_TestTimeSheetReject()
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                var client = new HttpClient();

                // act

                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = new Uri(ServiceURL + "RejectTimeSheet");
                client.DefaultRequestHeaders.Host = uri.Host;


                // Create the json object for approval
                TimeSheetSlim ts = new TimeSheetSlim()
                {
                    //CandidateId = 1133,
                    CandidateGuid = "C826EE30-8DDD-4A2B-9672-1F55715C2B7F",
                    TimeSheetId = 163630,
                    Note = "test"
                };

                var body = new TimeSheetAndSession() { SessionId = sessionId, CandidateTimeSheet = ts };

                // Serialize the object
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(TimeSheetAndSession)); //Create a Json Serializer for our type 

                // use the serializer to write the object to a MemoryStream 
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonSer.WriteObject(ms, body);
                    ms.Position = 0;

                    //use a Stream reader to construct the StringContent (Json) 
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        StringContent newContent = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync(uri, newContent).GetAwaiter().GetResult();

                        if (response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            Assert.IsTrue(true);
                        }
                    }
                }
            }
            else
            {
                Assert.Fail();
            }

        }


        [TestMethod]
        public void WCF_TestUpdateTimeSheet()
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                var client = new HttpClient();

                // act


                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = new Uri(ServiceURL + "UpdateTimeSheet");
                client.DefaultRequestHeaders.Host = uri.Host;

                // Create the json objects for update
                TimeSheet ts = new TimeSheet()
                {
                     AdjustmentInMinutes = 30,
                     //CandidateId = 77204,
                     Note = "tested by unit test",
                     CandidateGuid = "D415E688-7167-495D-AC2C-455D164AE197",
                     TimeSheetId = 194680,
                     JobStartDateTime = new DateTime(2016,7,18,7,0,0),
                     JobEndDateTime = new DateTime(2016,7,18,15,30,0)
                };


                var objectToSerialize = new RootObject2();
                objectToSerialize.candidateTimeSheet = ts;
                objectToSerialize.SessionId = sessionId;

                // Serialize the object
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(RootObject2)); //Create a Json Serializer for our type 

                // use the serializer to write the object to a MemoryStream 
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonSer.WriteObject(ms, objectToSerialize);
                    ms.Position = 0;

                    //use a Stream reader to construct the StringContent (Json) 
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        StringContent newContent = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync(uri, newContent).GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            Assert.IsTrue(true);
                        }
                    }
                }
            }
            else
            {
                Assert.Fail();
            }

        }

        public class RootObject3
        {
            public string sessionId { get; set; }
            public WorkTime workTime { get; set; }
        } 
        [TestMethod]
        public void WCF_TestAddTimeSheet()
        {
            //assemble
            var sessionId = this.CreateSession();
            if (sessionId != null)
            {
                var client = new HttpClient();

                // act

                // configure the approval web request 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = new Uri(ServiceURL + "AddCandidateWorkTime");
                client.DefaultRequestHeaders.Host = uri.Host;

                // Create the json objects for approval
                WorkTime ts = new WorkTime()
                {
                    //CandidateId = 77204,
                    JobOrderGuid = "83CBCA24-F878-4B39-9730-1CDF5A15EDB7",
                    NetWorkTimeInHours = 7.5m,
                    WorkDate = new DateTime(2016, 6, 5, 7, 0, 0),
                    Note = "test",
                    CandidateGuid = "92C7DD05-B354-4BEF-9F06-1762784DCF1A"
                };


                var objectToSerialize = new RootObject3();
                objectToSerialize.workTime = ts;
                objectToSerialize.sessionId = sessionId;

                // Serialize the object
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(RootObject3)); //Create a Json Serializer for our type 

                // use the serializer to write the object to a MemoryStream 
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonSer.WriteObject(ms, objectToSerialize);
                    ms.Position = 0;

                    //use a Stream reader to construct the StringContent (Json) 
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        StringContent newContent = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync(uri, newContent).GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            Assert.IsTrue(true);
                        }
                    }
                }

            }
            else
            {
                Assert.IsTrue(false);
            }

        }
     }

}
