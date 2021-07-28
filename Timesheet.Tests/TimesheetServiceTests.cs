using NUnit.Framework;
using System;
using Timesheet.Application.Services;
using Timesheet.Domain.Models;

namespace Timesheet.Tests
{
    class TimesheetServiceTests
    {
        [Test]
        public void TrackTime_ShouldReturnTrue()
        {
            // arrange
            var expectedLastName = "TestUser";
            UserSession.Sesions.Add(expectedLastName);

            var timeLog = new TimeLog()
            {
                Date = new DateTime(),
                WorkingHours = 1,
                LastName = expectedLastName,
                Comment = Guid.NewGuid().ToString()
            };

            var service = new TimesheetService();

            //act
            var result = service.TrackTime(timeLog);

            // assert
            Assert.IsTrue(result);
        }

        [TestCase(-1, "")]
        [TestCase(-1, null)]
        [TestCase(-1, "TestUser")]
        [TestCase(25, "")]
        [TestCase(25, null)]
        [TestCase(25, "TestUser")]
        [TestCase(4, "")]
        [TestCase(4, null)]
        [TestCase(4, "TestUser")]
        public void TrackTime_ShouldReturnFalse(int workingHours, string lastName)
        {
            // arrange

            var timeLog = new TimeLog()
            {
                Date = new DateTime(),
                WorkingHours = workingHours,
                LastName = lastName
            };

            var service = new TimesheetService();

            //act
            var result = service.TrackTime(timeLog);

            // assert
            Assert.IsFalse(result);
        }
    }
}
