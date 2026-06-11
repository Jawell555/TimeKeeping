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
        ShiftSchedule? GetEmployeeShift(int shiftID);
        TimeLogs? GetLastTimeIn(int employeeID);
        void UpdateTimeLog(TimeLogs log);
        bool AlreadyTimedIn(int employeeID);
        bool EmployeeExists(int Employee);
        List<TimeLogs> GetAllLogs();
        List<TimeLogs> GetEmployeeLogs(int employeeID);
        List<Employee> GetEmployees();
        List<ShiftSchedule> GetShifts();
        List<TimeLogs> GetLatestEmployeeLogs();
        TimeLogs? GetLatestEmployeeLogByID(int employeeID);
        void AddShiftSchedule(ShiftSchedule shift);
        int GenerateShiftID();
        void UpdateShiftSchedule(ShiftSchedule shift);
        void DeleteShiftSchedule(int shiftID);
        bool ShiftExists(int shiftID);
        List<TimeLogs> GetTimeLogsByDate(DateOnly date);
    }
}
