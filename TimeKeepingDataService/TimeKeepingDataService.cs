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
        public ShiftSchedule? GetEmployeeShift(Employee employee)
        {
            return _dataService.GetEmployeeShift(employee);
        }
        public TimeLogs? GetLogByDate(int employeeID, DateTime timeOutTime)
        {
            return _dataService.GetLogByDate(employeeID, timeOutTime);
        }
        public void UpdateTimeLog(TimeLogs log)
        {
            _dataService.UpdateTimeLog(log);
        }
        public bool AlreadyTimedIn(int employeeID, DateTime timeInTime)
        {
            return _dataService.AlreadyTimedIn(employeeID, timeInTime);
        }
        public bool EmployeeExists(int employeeID)
        {
            return _dataService.EmployeeExists(employeeID);
        }
        public List<TimeLogs> GetAllLogs()
        {
            return _dataService.GetAllLogs();
        }

    }
}
