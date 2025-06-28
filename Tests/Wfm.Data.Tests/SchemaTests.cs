using System;
using System.Data.Entity;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests
{
    [TestFixture]
    public class SchemaTests
    {
        [Test]
        public void Can_generate_schema()
        {
            Database.SetInitializer<WfmObjectContext>(null);
            var ctx = new WfmObjectContext("Test");
            string result = ctx.CreateDatabaseScript();
            result.ShouldNotBeNull();
            Console.Write(result);
        }
    }
}
