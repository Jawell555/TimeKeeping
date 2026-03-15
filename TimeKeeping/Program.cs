using System;
using System.ComponentModel;
using System.Data;
using System.Numerics;
using TimeKeepingAppService;
using TimeKeepingModels;

internal class Program
{
    static EmployeeAppService appService = new EmployeeAppService();
    static DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
    static int currentEmployeeID;

    static void Main(string[] args)
    {
        mainDisplay();
    }

    static void TimeInOutDisplay()
    {
        bool isAdmin = appService.IsAdmin(currentEmployeeID);
        Console.WriteLine($"\nWelcome, Employee {currentEmployeeID}");
        Console.WriteLine("\nDo you want to:\n" +
            "1. Time In\n" +
            "2. Time Out");

        if (isAdmin)
        {
            Console.WriteLine("3. View Logs");
        }

        Console.WriteLine("4. Exit");
        Console.Write("Select Option(1-4): ");

        int choice = Convert.ToInt32(Console.ReadLine());
        switch (choice)
        {
            case 4:
                Console.WriteLine("Exiting the program.");
                Environment.Exit(0);
                break;
            case 3:
                if (isAdmin)
                {
                    ViewLogs();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;
            case 1:
                TimeIn(currentEmployeeID, DateTime.Now);
                break;
            case 2:
                TimeOut(currentEmployeeID, DateTime.Now);
                break;
            default:
                Console.WriteLine("Invalid Selection. Try Again");
                break;
        }
    }
    static int employeeIdValidation()
    {
        if (!int.TryParse(Console.ReadLine(), out int currentEmployeeID))
        {
            Console.WriteLine("Invalid ID format. Please restart and enter a valid number.");
            return -1;
        }
        if (!appService.EmployeeExists(currentEmployeeID))
        {
            Console.WriteLine("Employee not found. Try Again, System Restarting.");
            return -1;
        }
        return currentEmployeeID;
    }
    static  void mainDisplay()
    {
        while (true)
        {
            Console.WriteLine("----- TIME KEEPING SYSTEM -----");
            Console.WriteLine($"{dateToday}");
            Console.Write("Enter EmployeeID: ");
            currentEmployeeID = employeeIdValidation();
            if (currentEmployeeID==-1)
            {
                continue;
            }
            TimeInOutDisplay();
        }
    }
    static void TimeIn(int employeeID, DateTime timeInTime)
    {
        if (!appService.EmployeeExists(employeeID))
        {
            Console.WriteLine("\nEmployee not found.");
            return;
        }
        if (appService.alreadyTimedIn(employeeID, timeInTime))
        {
            Console.WriteLine("\nYou already timed in.");
            return;

        }
        Employee employee = appService.GetEmployee(employeeID);
        ShiftSchedule shift = appService.GetShiftSchedule(employee);

        TimeSpan late = appService.calcLate(shift.ShiftStartTime, timeInTime);

        TimeLogs newLog = new TimeLogs { EmployeeID = employeeID, Date = DateOnly.FromDateTime(timeInTime), TimeIn = timeInTime, LateHours = late };

        appService.AddTimeLog(newLog);
        Console.WriteLine($"\nEmployee {employeeID} timed in at {timeInTime}. Late: {late:hh\\:mm\\:ss}\n");
    }
    static void TimeOut(int employeeID, DateTime timeOutTime)
    {

        TimeLogs log = appService.GetTimeLogs(employeeID, timeOutTime);
        if (log == null)
        {
            Console.WriteLine("\nYou must time in first.");
            return;
        }

        Employee employee = appService.GetEmployee(employeeID);
        ShiftSchedule shift = appService.GetShiftSchedule(employee);
        log.TimeOut = timeOutTime;
        log.WorkingHours = appService.calcWorkingHours(log.TimeIn, timeOutTime);

        if (timeOutTime > shift.ShiftEndTime)
        {
            log.OvertimeHours = appService.calcOvertime(shift.ShiftEndTime, timeOutTime);
        }
        else if (timeOutTime < shift.ShiftEndTime)
        {
            log.UndertimeHours = appService.calcUndertime(shift.ShiftEndTime, timeOutTime);
        }

        Console.WriteLine($"\nEmployee {employeeID} timed out at {timeOutTime}. Working Hours: {log.WorkingHours:hh\\:mm\\:ss}\n");
    }
    static void ViewLogs()
    {
        var timeLogs = appService.GetAllLogs();
        Console.WriteLine("\n-----TIME LOGS-----");
        if (!timeLogs.Any())
        {
            Console.WriteLine("No logs to display.\n");
            return;
        }

        foreach (var l in timeLogs)
        {
            Console.WriteLine($"Employee ID: {l.EmployeeID}, Date: {l.Date}, Time In: {l.TimeIn:HH\\:mm}, Time Out: {(l.TimeOut != DateTime.MinValue ? (l.TimeOut) : "Ongoing")}, Working Hours: {l.WorkingHours:hh\\:mm\\:ss}, Late: {l.LateHours:hh\\:mm\\:ss}, OT: {l.OvertimeHours:hh\\:mm\\:ss}");
        }
        Console.WriteLine("---------------------\n");
    }
}