using Moq;
using NUnit.Framework;
using System;
using Timesheet.Application.Services;
using Timesheet.Domain;
using Timesheet.Domain.Models;

namespace Timesheet.Tests
{
    class ReportServiceTests
    {
        [Test]
        public void GetEmployrrReport_ShouldReturnReport()
        {
            // arrange
            var timeSheetRepositoryMock = new Mock<ITimesheetRepository>();
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var expectedLastName = "Иванов2";
            // ставка за час = 60000 / 160 = 375
            // 20 * 8 * 375 + 10 * 8 * 375 * 2(дни переработки) + 1 * 375 * 2(1 час переработки в 1 дне)+5 * 8 * 375 (рабочие дни в следующем месяце) 
            var expectedTotal = 135750m;
            var expectedTotalHours = 281;

            employeeRepositoryMock
                .Setup(x => x.GetEmployee(It.Is<string>(y => y == expectedLastName)))
                .Returns(() => new StaffEmployee
                {
                    LastName = expectedLastName,
                    Salary = 60000m
                })
                .Verifiable();

            timeSheetRepositoryMock
                .Setup(x => x.GetTimeLogs(It.Is<string>(y => y == expectedLastName)))
                .Returns(() =>
                {
                    TimeLog[] timeLogs = new TimeLog[35];
                    DateTime dateTime = new DateTime(2020, 11, 1);
                    timeLogs[0] = new TimeLog
                    {
                        LastName = expectedLastName,
                        Comment = Guid.NewGuid().ToString(),
                        Date = dateTime,
                        WorkingHours = 9
                    };
                    for (int i = 1; i < timeLogs.Length; i++)
                    {
                        dateTime = dateTime.AddDays(1);
                        timeLogs[i] = new TimeLog
                        {
                            LastName = expectedLastName,
                            Comment = Guid.NewGuid().ToString(),
                            Date = dateTime,
                            WorkingHours = 8
                        };
                    }
                    return timeLogs;
                })
                .Verifiable();

            var service = new ReportService(timeSheetRepositoryMock.Object, employeeRepositoryMock.Object);

            // act
            var result = service.GetEmployeeReport(expectedLastName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedLastName, result.LastName);

            Assert.IsNotNull(result.TimeLogs);
            Assert.IsNotEmpty(result.TimeLogs);

            Assert.AreEqual(expectedTotal, result.Bill);
            Assert.AreEqual(expectedTotalHours, result.TotalHours);
        }

        [Test]
        public void GetEmployrrReport_WithoutTimeLog_ShouldReturnReport()
        {
            // arrange
            var timeSheetRepositoryMock = new Mock<ITimesheetRepository>();
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var expectedLastName = "Иванов";
            var expectedTotal = 0m;
            var expectedTotalHours = 0m;

            employeeRepositoryMock
                .Setup(x => x.GetEmployee(It.Is<string>(y => y == expectedLastName)))
                .Returns(() => new StaffEmployee
                {
                    LastName = expectedLastName,
                    Salary = 70000m
                })
                .Verifiable();

            timeSheetRepositoryMock
                .Setup(x => x.GetTimeLogs(It.Is<string>(y => y == expectedLastName)))
                .Returns(() => new TimeLog[0])
                .Verifiable();

            var service = new ReportService(timeSheetRepositoryMock.Object, employeeRepositoryMock.Object);

            // act
            var result = service.GetEmployeeReport(expectedLastName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedLastName, result.LastName);

            Assert.IsNotNull(result.TimeLogs);
            Assert.IsEmpty(result.TimeLogs);

            Assert.AreEqual(expectedTotal, result.Bill);
            Assert.AreEqual(expectedTotalHours, result.TotalHours);
        }
    }
}
