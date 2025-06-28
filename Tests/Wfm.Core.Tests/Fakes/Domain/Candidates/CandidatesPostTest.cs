using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using NUnit.Framework;
using Wfm.Tests;
using Wfm.Core.Domain.Companies;


namespace Wfm.Core.Tests.Domain.Candidates
{
   [TestFixture]
    public class CandidatesPostTest
    {
        [Test]
        public void Can_parse()
        {
            var candidate = new Candidate()
            {
                
                JobTitle = "General Labour"
            };


            //var companyContact = new CompanyContact()
            //{
            //    FirstName = " Dennis"
            //};

            candidate.JobTitle.ShouldEqual("General Labour");
            //companyContact.FirstName.ShouldEqual("Dennis");
        }
    }
}
