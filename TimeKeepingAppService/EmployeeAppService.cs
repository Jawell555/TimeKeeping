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
        public bool IsAdmin(int employeeID)
        {
            Employee employee = timeKeepingDataService.dummyEmployee.FirstOrDefault(e => e.EmployeeID == employeeID);
            return employee != null && employee.IsAdmin;
        }
        public void TimeIn(int employeeID, DateTime timeInTime)
        {
            Employee employee = timeKeepingDataService.dummyEmployee.FirstOrDefault(e => e.EmployeeID == employeeID);
            if (employee == null) 
            {
                Console.WriteLine("Employee not found.");
                return;
            }
            bool alreadyTimedIn = timeKeepingDataService.LoggedTimes.Any(log => log.EmployeeID == employeeID && log.Date == DateOnly.FromDateTime(timeInTime) && log.TimeOut == DateTime.MinValue);
            if (alreadyTimedIn)
            {
                Console.WriteLine("You already timed in.");
                return;
            }
            ShiftSchedule shift = timeKeepingDataService.FixedSchedule.FirstOrDefault(s => s.ShiftID == employee.ShiftID);

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

                TimeLogs log = timeKeepingDataService.LoggedTimes.FirstOrDefault(l => l.EmployeeID == employeeID && l.Date == DateOnly.FromDateTime(timeOutTime) && l.TimeOut == DateTime.MinValue);
                if (log == null)
                {
                    Console.WriteLine("You must time in first.");
                    return;
                }
                
                Employee employee = timeKeepingDataService.dummyEmployee.First(e => e.EmployeeID == employeeID);
                ShiftSchedule shift = timeKeepingDataService.FixedSchedule.First(s => s.ShiftID == employee.ShiftID);
                log.TimeOut = timeOutTime;
                log.WorkingHours = timeOutTime - log.TimeIn;

            if (timeOutTime > shift.ShiftEndTime)
            {
                log.OvertimeHours = timeOutTime - shift.ShiftEndTime;
            }
            else if (timeOutTime < shift.ShiftEndTime)
            {
                log.UndertimeHours = shift.ShiftEndTime - timeOutTime;
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
                    Console.WriteLine($"Employee ID: {l.EmployeeID}, Date: {l.Date}, Time In: {l.TimeIn:hh\\:mm}, Time Out: {(l.TimeOut!= DateTime.MinValue? (l.TimeOut): "Ongoing" )}, Working Hours: {l.WorkingHours:hh\\:mm}, Late: {l.LateHours:hh\\:mm}, OT: {l.OvertimeHours:hh\\:mm}");
                }
            Console.WriteLine("---------------------\n");
            


        }
    }
}
