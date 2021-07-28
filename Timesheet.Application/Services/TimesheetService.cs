using System;
using System.Collections.Generic;
using Timesheet.Domain;
using Timesheet.Domain.Models;

namespace Timesheet.Application.Services
{
    public class TimesheetService : ITimesheetService
    {
        public bool TrackTime(TimeLog timeLog)
        {
            var isValid = timeLog.WorkingHours > 0 && timeLog.WorkingHours <= 24
                          && !string.IsNullOrWhiteSpace(timeLog.LastName);

            isValid = isValid && UserSession.Sesions.Contains(timeLog.LastName);

            if (!isValid)
            {
                return false;
            }

            Timesheets.TimeLogs.Add(timeLog);

            return true;
        }
    }

    public static class Timesheets
    {
        public static List<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
    }
}
