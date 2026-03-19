using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeepingModels;

namespace TimeKeepingManagementDataService
{
    public interface ITimeKeepingDataService
    {
        void Add(Employee employee);
        void Add(ShiftSchedule shift);
        void Add(TimeLogs timeLog);
        Employee? GetEmployeeByID(int employeeID);
        ShiftSchedule? GetEmployeeShift(Employee employee);
        TimeLogs? GetLogByDate(int employeeID, DateTime timeOutTime);
        void UpdateTimeLog(TimeLogs log);
        bool AlreadyTimedIn(int employeeID, DateTime timeInTime);
        bool EmployeeExists(int Employee);
        List<TimeLogs> GetAllLogs();

    }
}
