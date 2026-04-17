using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeepingModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeKeepingManagementDataService
{
    public class EmployeeShiftingInMemory : ITimeKeepingDataService
    {
        public List<Employee> dummyEmployee = new List<Employee>();
        public List<ShiftSchedule> FixedSchedule = new List<ShiftSchedule>();
        public List<TimeLogs> LoggedTimes = new List<TimeLogs>();

        public EmployeeShiftingInMemory()
        {
            TimeOnly shiftStart1 = new TimeOnly(6, 0);
            TimeOnly shiftStart2 = shiftStart1.AddHours(8);
            TimeOnly shiftStart3 = shiftStart2.AddHours(8);

            ShiftSchedule morningShift = new ShiftSchedule { ShiftID = 1, ShiftName = "Morning",ShiftStartTime = shiftStart1, ShiftEndTime = shiftStart1.AddHours(8) };
            ShiftSchedule afternoonShift = new ShiftSchedule { ShiftID = 2, ShiftName = "Afternoon", ShiftStartTime = shiftStart2, ShiftEndTime = shiftStart2.AddHours(8) };
            ShiftSchedule nightShift = new ShiftSchedule { ShiftID = 3, ShiftName = "Night", ShiftStartTime = shiftStart3, ShiftEndTime = shiftStart3.AddHours(8) };

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
        public bool AlreadyTimedIn(int employeeID)
        {
            return LoggedTimes.Any(l => l.EmployeeID == employeeID && l.TimeOut == null);
        }
        public bool EmployeeExists(int Employee)
        {
            return dummyEmployee.Any(e => e.EmployeeID == Employee);

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
        public TimeLogs? GetLastLog(int employeeID)
        {
            return LoggedTimes.FirstOrDefault(l => l.EmployeeID == employeeID && l.TimeOut == null);
        
        }
        public List<TimeLogs> GetEmployeeLogs (int employeeID)
        {
            return LoggedTimes.FindAll(l => l.EmployeeID == employeeID);
        }
        public void UpdateTimeLog(TimeLogs log)
        {
            var existingLog = LoggedTimes.FirstOrDefault(l => l.EmployeeID == log.EmployeeID && l.TimeOut == null);
            if (existingLog != null)
            {
                existingLog.TimeIn = log.TimeIn;
                existingLog.TimeOut = log.TimeOut;
                existingLog.WorkingHours = log.WorkingHours;
                existingLog.LateHours = log.LateHours;
                existingLog.OvertimeHours = log.OvertimeHours;
                existingLog.UndertimeHours = log.UndertimeHours;
            }
        }
        public List<TimeLogs> GetAllLogs()
        {
                       return LoggedTimes;
        }
    }
}
