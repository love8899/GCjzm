﻿using System.Linq;
using Wfm.Core.Infrastructure;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Core.Tests.Infrastructure
{
    [TestFixture]
    public class TypeFinderTests
    {
        [Test]
        public void TypeFinder_Benchmark_Findings()
        {
            var finder = new AppDomainTypeFinder();

            var type = finder.FindClassesOfType<ISomeInterface>();
            type.Count().ShouldEqual(1);
            typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).ShouldBeTrue();
        }

        public interface ISomeInterface
        {
        }

        public class SomeClass : ISomeInterface
        {
        }
    }
}
