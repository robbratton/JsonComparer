using System;
using System.Runtime.InteropServices;
using JsonComparer;
using NUnit.Framework;

namespace JsonCompareTests
{
    [TestFixture]
    public static class ComparerTests
    {
        [Test]
        public static void CompareFiles_ReturnsExpectedResults()
        {
            var result = Comparer.CompareFiles(
                "TestFiles\\Microservice-Healthwise-DevBranch-CI-2.json",
                "TestFiles\\Microservice-Healthwise-DevBranch-CI-1.json"
                );

            Assert.That(result, Is.Not.Null);
        }
    }
}
