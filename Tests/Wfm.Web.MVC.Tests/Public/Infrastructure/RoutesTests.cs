using System;
using Wfm.Web.Controllers;
using NUnit.Framework;

namespace Wfm.Web.MVC.Tests.Public.Infrastructure
{
    [TestFixture]
    public class RoutesTests : RoutesTestsBase
    {
        [Test]
        public void Default_route()
        {
            "~/".ShouldMapTo<HomeController>(c => c.Index());
        }

        [Test]
        public void Blog_routes()
        {
            //TODO why does it pass null instead of "new BlogPagingFilteringModel()" as it's done in real application? The same is about issue is in the other route test methods
            "~/blog/".ShouldMapTo<BlogController>(c => c.List(null, 1));
            //"~/blog/rss/1".ShouldMapTo<BlogController>(c => c.ListRss(1));
            //"~/blog/2/".ShouldMapTo<BlogController>(c => c.BlogPost(2));
            //"~/blog/2/test-se-name".ShouldMapTo<BlogController>(c => c.BlogPost(2));
            //TODO validate properties such as 'Tag' or 'Month' in the passed BlogPagingFilteringModel. The same is about issue is in the other route test methods
            //"~/blog/tag/sometag".ShouldMapTo<BlogController>(c => c.List(new BlogPagingFilteringModel() { Tag = "sometag" }));
            //"~/blog/month/3".ShouldMapTo<BlogController>(c => c.List(new BlogPagingFilteringModel() { Month = "4" }));
        }

        [Test]
        public void Common_routes()
        {
        }

    }
}
