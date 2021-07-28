using System;
using System.Collections.Generic;
using System.Linq;
using Timesheet.Domain;

namespace Timesheet.Application.Services
{
    public class AuthService : IAuthService
    {
        public AuthService()
        {
            Employees = new List<string>
            {
                "Иванов",
                "Петров",
                "Сидоров"
            };
        }

        public List<string> Employees { get; private set; }

        public bool Login(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            var isEmployeeExist = Employees.Contains(lastName);

            if (isEmployeeExist)
            {
                UserSession.Sesions.Add(lastName);
            }

            return isEmployeeExist;
        }
    }

    public static class UserSession
    {
        public static HashSet<string> Sesions { get; set; } = new HashSet<string>();
    }
}
