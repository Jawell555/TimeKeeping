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
        ShiftingScheduleDataService timeKeepingDataService = new ShiftingScheduleDataService();

        public bool EmployeeExists(int Employee)
        {
            return timeKeepingDataService.dummyEmployee.Any(e => e.EmployeeID == Employee);
           
        }
        public Employee? GetEmployee(int employeeID)
        {
            return timeKeepingDataService.GetEmployeeByID(employeeID);
        }
        public ShiftSchedule? GetShiftSchedule(Employee employee)
        {
            return timeKeepingDataService.GetEmployeeShift(employee);
        }
        public bool alreadyTimedIn(int employeeID, DateTime timeInTime)
        {
            return timeKeepingDataService.LoggedTimes.Any(l => l.EmployeeID == employeeID && l.Date == DateOnly.FromDateTime(timeInTime) && l.TimeOut == DateTime.MinValue);
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

        public void TimeIn(int employeeID, DateTime timeInTime)
        {
            if (!EmployeeExists(employeeID)) 
            {
                Console.WriteLine("Employee not found.");
                return;
            }
            if (alreadyTimedIn(employeeID,timeInTime))
            {
                Console.WriteLine("You already timed in.");
                return;
            }
            Employee employee = GetEmployee(employeeID);
            ShiftSchedule shift = GetShiftSchedule(employee);

            TimeSpan late = TimeSpan.Zero;
            if (timeInTime > shift.ShiftStartTime)
            {
                late = timeInTime - shift.ShiftStartTime;
            }
            
            TimeLogs newLog = new TimeLogs { EmployeeID = employeeID,Date = DateOnly.FromDateTime(timeInTime), TimeIn = timeInTime, LateHours = late};

            timeKeepingDataService.LoggedTimes.Add(newLog);
            Console.WriteLine($"Employee {employeeID} timed in at {timeInTime}. Late: {late}");
        }

            public void TimeOut(int employeeID, DateTime timeOutTime)
            {

                TimeLogs log = GetTimeLogs(employeeID, timeOutTime);
            if (log == null)
                {
                    Console.WriteLine("You must time in first.");
                    return;
                }
                
            Employee employee = GetEmployee(employeeID);
            ShiftSchedule shift = GetShiftSchedule(employee);
            log.TimeOut = timeOutTime;
            log.WorkingHours = timeOutTime - log.TimeIn;

            if (timeOutTime > shift.ShiftEndTime)
            {
                log.OvertimeHours = calcOvertime(shift.ShiftEndTime, timeOutTime);
            }
            else if (timeOutTime < shift.ShiftEndTime)
            {
                log.UndertimeHours = calcUndertime(shift.ShiftEndTime, timeOutTime);
            }

            Console.WriteLine($"Employee {employeeID} timed out at {timeOutTime}. Working Hours: {log.WorkingHours}");
        }
        public void ViewLogs()
        {
            Console.WriteLine("-----TIME LOGS-----");
            if (!timeKeepingDataService.LoggedTimes.Any())
            {
                Console.WriteLine("No logs to display.\n");
                return;
            }

            foreach (var l in timeKeepingDataService.LoggedTimes)
            {
                Console.WriteLine($"Employee ID: {l.EmployeeID}, Date: {l.Date}, Time In: {l.TimeIn:hh\\:mm}, Time Out: {(l.TimeOut != DateTime.MinValue ? (l.TimeOut) : "Ongoing")}, Working Hours: {l.WorkingHours:hh\\:mm}, Late: {l.LateHours:hh\\:mm}, OT: {l.OvertimeHours:hh\\:mm}");
            }
            Console.WriteLine("---------------------\n");
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
    }
}
