using TimeKeepingManagementDataService;
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

        TimeKeepingDataService timeKeepingDataService = new TimeKeepingDataService(new EmployeeShiftingDBData());


        public Employee? GetEmployee(int employeeID)
        {
            return timeKeepingDataService.GetEmployeeByID(employeeID);
        }
        public ShiftSchedule? GetShiftSchedule(int employeeID)
        {
            return timeKeepingDataService.GetEmployeeShift(employeeID);
        }
        public List<Employee> GetAllEmployees()
        {
            return timeKeepingDataService.GetEmployees();
        }
        public List<ShiftSchedule> GetAllShiftSchedules()
        {
            return timeKeepingDataService.GetShifts();
        }


        public bool EmployeeExists(int employeeID)
        {
            return timeKeepingDataService.EmployeeExists(employeeID);
        }
        public bool AlreadyTimedIn(int employeeID)
        {
            return timeKeepingDataService.AlreadyTimedIn(employeeID);
        }
        public bool IsAdmin(int employeeID)
        {
            Employee employee = GetEmployee(employeeID);
            return employee != null && employee.IsAdmin;
        }
        public TimeLogs? GetLastTimeIn(int employeeID)
        {
            return timeKeepingDataService.GetLastTimeIn(employeeID);
        }
        public void UpdateLog(TimeLogs log)
        {
            timeKeepingDataService.UpdateTimeLog(log);
        }
        public void AddTimeLog(TimeLogs log)
        {
            timeKeepingDataService.AddTimeLog(log);
        }
        public List<TimeLogs> GetAllLogs()
        {
            return timeKeepingDataService.GetAllLogs();
        }
        public List<TimeLogs> GetEmployeeLogs(int employee)
        {
            return timeKeepingDataService.GetEmployeeLogs(employee);
        }
        public TimeSpan calcUndertime(TimeSpan endTime, TimeSpan timeOut)
        {
            TimeSpan Undertime = endTime - timeOut;
            return Undertime;
        }
        public TimeSpan calcOvertime(TimeSpan endTime, TimeSpan timeOut)
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
        public List<TimeLogs> GetLatestEmployeeLogs()
        {
            return timeKeepingDataService.GetLatestEmployeeLogs();
        }
        public TimeLogs? GetLatestEmployeeLogByID(int employeeID)
        {
            return timeKeepingDataService.GetLatestEmployeeLogByID(employeeID);
        }
        public void AddShiftSchedule(ShiftSchedule shift)
        {
            timeKeepingDataService.AddShiftSchedule(shift);
        }
        public int GenerateShiftID()
        {
            return timeKeepingDataService.GenerateShiftID();
        }
        public void UpdateShiftSchedule(ShiftSchedule shift)
        {
            timeKeepingDataService.UpdateShiftSchedule(shift);
        }
        public void DeleteShiftSchedule(int shiftID)
        {
            timeKeepingDataService.DeleteShiftSchedule(shiftID);
        }
    }
}
