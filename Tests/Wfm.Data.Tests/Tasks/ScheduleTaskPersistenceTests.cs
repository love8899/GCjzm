﻿using System;
using Wfm.Core.Domain.Tasks;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Tasks
{
    [TestFixture]
    public class ScheduleTaskPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_scheduleTask()
        {
            var scheduleTask = new ScheduleTask
                               {
                                   Name = "Task 1",
                                   Seconds = 1,
                                   Type = "some type 1",
                                   IsActive = true,
                                   StopOnError = true,
                                   LastStartUtc = new DateTime(2010, 01, 01),
                                   LastEndUtc = new DateTime(2010, 01, 02),
                                   LastSuccessUtc= new DateTime(2010, 01, 03),
                               };

            var fromDb = SaveAndLoadEntity(scheduleTask);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Task 1");
            fromDb.Seconds.ShouldEqual(1);
            fromDb.Type.ShouldEqual("some type 1");
            fromDb.IsActive.ShouldEqual(true);
            fromDb.StopOnError.ShouldEqual(true);
            fromDb.LastStartUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.LastEndUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.LastSuccessUtc.ShouldEqual(new DateTime(2010, 01, 03));
        }
    }
}