using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeepingModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeKeepingDataService
{
    public class ShiftingScheduleDataService
    {
        public List<Employee> dummyEmployee = new List<Employee>();
        public List<ShiftSchedule> FixedSchedule = new List<ShiftSchedule>();
        public List<TimeLogs> LoggedTimes = new List<TimeLogs>();

        public ShiftingScheduleDataService()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateTime shiftStart1 = today.ToDateTime(new TimeOnly(6, 0));
            DateTime shiftStart2 = shiftStart1.AddHours(8);
            DateTime shiftStart3 = shiftStart2.AddHours(8);

            ShiftSchedule morningShift = new ShiftSchedule { ShiftID = 1, ShiftStartTime = shiftStart1, ShiftEndTime = shiftStart1.AddHours(8) };
            ShiftSchedule afternoonShift = new ShiftSchedule { ShiftID = 2, ShiftStartTime = shiftStart2, ShiftEndTime = shiftStart2.AddHours(8) };
            ShiftSchedule nightShift = new ShiftSchedule { ShiftID = 3, ShiftStartTime = shiftStart3, ShiftEndTime = shiftStart3.AddHours(8) };

            FixedSchedule.Add(morningShift);
            FixedSchedule.Add(afternoonShift);
            FixedSchedule.Add(nightShift);

            Employee admin = new Employee { EmployeeID = 0, ShiftID = 1, IsAdmin = true };
            Employee employee1 = new Employee { EmployeeID = 1, ShiftID = 2, IsAdmin = false};
            Employee employee2 = new Employee { EmployeeID = 2, ShiftID = 3, IsAdmin = false};

            dummyEmployee.Add(admin);
            dummyEmployee.Add(employee1);
            dummyEmployee.Add(employee2);
        }
        public void Add(Employee employee)
        {
            dummyEmployee.Add(employee);
        }
        public void Add(ShiftSchedule shift)
        {
            FixedSchedule.Add(shift);
        }
        public void Add(TimeLogs timeLog)
        {
            LoggedTimes.Add(timeLog);
        }
        public Employee? GetEmployeeByID(int employeeID)
        {
            return dummyEmployee.FirstOrDefault(e => e.EmployeeID == employeeID);
        }
        public ShiftSchedule? GetEmployeeShift(Employee employee)
        {
            return FixedSchedule.FirstOrDefault(s => s.ShiftID == employee.ShiftID);
        }
        public TimeLogs? GetLogByDate(int employeeID, DateTime timeOutTime)
        {
            return LoggedTimes.FirstOrDefault(l => l.EmployeeID == employeeID && l.Date == DateOnly.FromDateTime(timeOutTime) && l.TimeOut == DateTime.MinValue);
        
        }
        public List<TimeLogs> GetAllLogs()
        {
                       return LoggedTimes;
        }
    }
}
