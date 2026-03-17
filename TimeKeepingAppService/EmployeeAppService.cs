using TimeKeepingDataService;
using TimeKeepingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeepingAppService
{
    
    public class EmployeeAppService
    {
        //ShiftingScheduleDataService timeKeepingDataService = new ShiftingScheduleDataService();
        EmployeeShiftingJsonData timeKeepingDataService = new EmployeeShiftingJsonData();
        public EmployeeAppService()
        {
            EmployeeShiftingJsonData jsonData = new EmployeeShiftingJsonData();
        }
        //public bool EmployeeExists(int Employee)
        //{
        //    return timeKeepingDataService.dummyEmployee.Any(e => e.EmployeeID == Employee);
           
        //}
        public Employee? GetEmployee(int employeeID)
        {
            return timeKeepingDataService.GetEmployeeByID(employeeID);
        }
        public ShiftSchedule? GetShiftSchedule(Employee employee)
        {
            return timeKeepingDataService.GetEmployeeShift(employee);
        }
        //public bool alreadyTimedIn(int employeeID, DateTime timeInTime)
        //{
        //    return timeKeepingDataService.LoggedTimes.Any(l => l.EmployeeID == employeeID && l.Date == DateOnly.FromDateTime(timeInTime) && l.TimeOut == DateTime.MinValue);
        //}

        public bool EmployeeExists(int employeeID)
        {
            return timeKeepingDataService.EmployeeExists(employeeID);
        }
        public bool AlreadyTimedIn(int employeeID, DateTime timeInTime)
        {
            return timeKeepingDataService.AlreadyTimedIn(employeeID, timeInTime);
        }
        public bool IsAdmin(int employeeID)
        {
            Employee employee= GetEmployee(employeeID);
            return employee != null && employee.IsAdmin;
        }
        public TimeLogs? GetTimeLogs(int employeeID, DateTime date)
        {
            return timeKeepingDataService.GetLogByDate(employeeID, date);
        }
        public void UpdateLog(TimeLogs log)
        {
            timeKeepingDataService.UpdateTimeLog(log);
        }
        public void AddTimeLog(TimeLogs log)
        {
            timeKeepingDataService.Add(log);
        }
        public List<TimeLogs> GetAllLogs()
        {
            return timeKeepingDataService.GetAllLogs();
        }
        public TimeSpan calcUndertime(DateTime endTime, DateTime timeOut)
        {
            TimeSpan Undertime = endTime - timeOut;
            return Undertime;
        }
        public TimeSpan calcOvertime(DateTime endTime, DateTime timeOut)
        {
            TimeSpan Overtime = timeOut - endTime;
            return Overtime;
        }
        public TimeSpan calcLate(DateTime startTime, DateTime timeIn)
        {
            TimeSpan late = TimeSpan.Zero;
            if (timeIn > startTime)
            {
                late = timeIn - startTime;
            }
            return late;
        }
        public TimeSpan calcWorkingHours(DateTime timeIn, DateTime timeOut)
        {
            return timeOut - timeIn;
        }
    }
}
