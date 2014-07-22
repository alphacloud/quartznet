using System;

using NUnit.Framework;

using Quartz.Impl;

namespace Quartz.Tests.Unit
{
    [TestFixture]
    public class ConfigurationBuilderTest
    {
        [SetUp]
        public void SetUp()
        {
            builder = ConfigurationBuilder.Create();
        }

        private ConfigurationBuilder builder;

        [Test]
        public void WithBatchTimeWindow_Should_SpecifyTimeInMilliseconds()
        {
            var cfg = builder.WithBatchTimeWindow(TimeSpan.FromMilliseconds(500))
                .Build();

            Assert.AreEqual("500", cfg[StdSchedulerFactory.PropertySchedulerBatchTimeWindow]);
        }


    }
}