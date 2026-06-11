using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeepingModels;

namespace TimeKeepingManagementDataService
{
    public class TimeKeepingDataService
    {
        ITimeKeepingDataService _dataService;
        public TimeKeepingDataService(ITimeKeepingDataService accountDataService)
        {
            _dataService = accountDataService;
        }
        public void AddEmployee(Employee employee)
        {
            _dataService.Add(employee);
        }
        public void AddShift(ShiftSchedule shift)
        {
            _dataService.Add(shift);
        }
        public void AddTimeLog(TimeLogs timeLog)
        {
            _dataService.Add(timeLog);
        }
        public Employee? GetEmployeeByID(int employeeID)
        {
            return _dataService.GetEmployeeByID(employeeID);
        }
        public ShiftSchedule? GetEmployeeShift(int employeeID)
        {
            return _dataService.GetEmployeeShift(employeeID);
        }
        public TimeLogs? GetLastTimeIn(int employeeID)
        {
            return _dataService.GetLastTimeIn(employeeID);
        }
        public void UpdateTimeLog(TimeLogs log)
        {
            _dataService.UpdateTimeLog(log);
        }
        public bool AlreadyTimedIn(int employeeID)
        {
            return _dataService.AlreadyTimedIn(employeeID);
        }
        public bool EmployeeExists(int employeeID)
        {
            return _dataService.EmployeeExists(employeeID);
        }
        public List<TimeLogs> GetAllLogs()
        {
            return _dataService.GetAllLogs();
        }
        public List<TimeLogs> GetEmployeeLogs(int employeeID)
        {
            return _dataService.GetEmployeeLogs(employeeID);
        }
        public List<Employee> GetEmployees()
        {
            return _dataService.GetEmployees();
        }
        public List<ShiftSchedule> GetShifts()
        {
            return _dataService.GetShifts();
        }
        public List<TimeLogs> GetLatestEmployeeLogs()
        {
            return _dataService.GetLatestEmployeeLogs();
        }
        public TimeLogs? GetLatestEmployeeLogByID(int employeeID)
        {
            return _dataService.GetLatestEmployeeLogByID(employeeID);
        }
        public void AddShiftSchedule(ShiftSchedule shift)
        {
            _dataService.AddShiftSchedule(shift);
        }
        public int GenerateShiftID()
        {
            return _dataService.GenerateShiftID();
        }
        public void UpdateShiftSchedule(ShiftSchedule shift)
        {
            _dataService.UpdateShiftSchedule(shift);
        }
        public void DeleteShiftSchedule(int shiftID)
        {
            _dataService.DeleteShiftSchedule(shiftID);
        }
        public bool ShiftExists(int shiftID)
        {
            return _dataService.ShiftExists(shiftID);
        }
        public List<TimeLogs> GetTimeLogsByDate(DateOnly date)
        {
            return _dataService.GetTimeLogsByDate(date);
        }
    }
}
