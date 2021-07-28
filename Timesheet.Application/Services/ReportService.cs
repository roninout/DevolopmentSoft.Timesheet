using System;
using System.Linq;
using Timesheet.Domain;
using Timesheet.Domain.Models;

namespace Timesheet.Application.Services
{
    public class ReportService
    {
        private const decimal MAX_WORKING_HOURS_PER_MONTH = 160m;
        private readonly ITimesheetRepository _timeSheetRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ReportService(ITimesheetRepository timesheetRepository, IEmployeeRepository employeeRepository)
        {
            _timeSheetRepository = timesheetRepository;
            _employeeRepository = employeeRepository;
        }

        public EmployeeReport GetEmployeeReport(string LastName)
        {
            var employee = _employeeRepository.GetEmployee(LastName);
            var timeLogs = _timeSheetRepository.GetTimeLogs(employee.LastName);

            if (timeLogs == null || timeLogs.Length == 0)
            {
                return new EmployeeReport
                {
                    Bill = 0,
                    TimeLogs = new System.Collections.Generic.List<TimeLog>(),
                    TotalHours = 0,
                    LastName = employee.LastName
                };
            }

            var totalHours = timeLogs.Sum(x => x.WorkingHours);
            int monthHours = timeLogs[0].WorkingHours;
            decimal billPerHour = employee.Salary / MAX_WORKING_HOURS_PER_MONTH;
            decimal bill = monthHours * billPerHour;

            for (int i = 1; i < timeLogs.Length; i++)
            {
                int dayHours = timeLogs[i].WorkingHours;

                if (timeLogs[i].Date.Month != timeLogs[i - 1].Date.Month)
                {
                    monthHours = 0;
                }

                monthHours += dayHours;

                if (monthHours <= MAX_WORKING_HOURS_PER_MONTH)
                {
                    bill += timeLogs[i].WorkingHours * billPerHour;
                }
                else if (monthHours < (MAX_WORKING_HOURS_PER_MONTH + 8))
                {
                    var overWorkHours = monthHours - MAX_WORKING_HOURS_PER_MONTH;
                    var simpleWorkHours = dayHours - overWorkHours;
                    bill += simpleWorkHours * billPerHour;
                    bill += overWorkHours * billPerHour * 2;
                }
                else
                {
                    bill += timeLogs[i].WorkingHours * billPerHour * 2;
                }
            }

            return new EmployeeReport
            {
                LastName = employee.LastName,
                TimeLogs = timeLogs.ToList(),
                Bill = bill,
                TotalHours = totalHours
            };
        }
    }
}
