using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.Common
{
    public interface IWebService
    {
        string PublishJobOrder(JobOrder jo, string jobBoards);
        void CallMonsterWebService(string url, JobOrder jo, string userName, string password, int boardId,out string error);
        HttpWebRequest CreateWebRequest(string url);

        XmlDocument CreateSoapEnvelope(JobOrder jobOrder, string userName, string password, int boardId, out string errors);
        void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest);
        bool ValidateJobXML(XmlDocument document, out string errorMessage);
    }
}
