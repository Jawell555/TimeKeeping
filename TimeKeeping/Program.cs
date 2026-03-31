using System;
using System.ComponentModel;
using System.Data;
using System.Numerics;
using System.Xml.Serialization;
using TimeKeepingAppService;
using TimeKeepingModels;

internal class Program
{
    static EmployeeAppService appService = new EmployeeAppService();
    static DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
    static int currentEmployeeID;
    static DateOnly today = DateOnly.FromDateTime(DateTime.Now);

    static void Main(string[] args)
    {
        mainDisplay();
    }

    static void TimeInOutDisplay()
    {
        bool isAdmin = appService.IsAdmin(currentEmployeeID);
        Console.WriteLine($"\nWelcome, Employee {currentEmployeeID}");
        bool loop = true;
        while (loop == true) { 
        Console.WriteLine("\nDo you want to:\n" +
            "1. Time In\n" +
            "2. Time Out" +
            "\n3. View Logs");

        if (isAdmin)
        {
            Console.WriteLine("4. View All Logs");
        }

        Console.WriteLine("5. Back");
            Console.WriteLine("6. Exit");
            Console.Write("Select Option(1-5): ");

        int choice = Convert.ToInt32(Console.ReadLine());
        switch (choice)
        {
            case 6:
                Console.WriteLine("Exiting the program.");
                Environment.Exit(0);
                break;
            case 5:
                    Console.WriteLine("Successfully retuned.\n");
                    break;
            case 4:
                if (isAdmin)
                {
                    ViewAllLogs();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;
            case 3:
                ViewLogs();
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
            loop = choiceValidation(choice, isAdmin);
        }
    }
    static bool choiceValidation(int choice, bool isAdmin)
    {
        if ((choice == 1 || choice == 2 || choice == 3||choice==5) && (isAdmin == false))
        {
            return false;
        }else if ((choice == 1 || choice == 2 || choice == 3 || choice == 4||choice==5) && (isAdmin == true))
        {
            return false;
        }
        else
        {
            return true;
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
        if (appService.AlreadyTimedIn(employeeID, timeInTime))
        {
            Console.WriteLine("\nYou already timed in.");
            return;

        }
        Employee employee = appService.GetEmployee(employeeID);
        ShiftSchedule shift = appService.GetShiftSchedule(employee);
        DateTime start = today.ToDateTime(shift.ShiftStartTime);
        TimeSpan late = appService.calcLate(start, timeInTime);

        TimeLogs newLog = new TimeLogs { EmployeeID = employeeID, ShiftName = shift.ShiftName ,Date = DateOnly.FromDateTime(timeInTime), TimeIn = timeInTime, LateHours = late };

        appService.AddTimeLog(newLog);
        Console.WriteLine($"\nEmployee {employeeID}, is {shift.ShiftName} shift. Timed in at {timeInTime}. Late: {late:hh\\:mm\\:ss}\n");
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
        DateTime end = today.ToDateTime(shift.ShiftEndTime);
        log.TimeOut = timeOutTime;
        log.WorkingHours = appService.calcWorkingHours(log.TimeIn, timeOutTime);
        TimeSpan constant = shift.ShiftEndTime - shift.ShiftStartTime;

        if (log.WorkingHours > constant)
        {
            log.OvertimeHours = appService.calcOvertime(constant, log.WorkingHours);
        }
        else if (log.WorkingHours < constant)
        {
            log.UndertimeHours = appService.calcUndertime(constant, log.WorkingHours);
        }
        appService.UpdateLog(log);
        Console.WriteLine($"\nEmployee {employeeID} timed out at {timeOutTime}. Working Hours: {log.WorkingHours:hh\\:mm\\:ss}\n");
    }
    static void ViewAllLogs()
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
            Console.WriteLine($"Employee ID: {l.EmployeeID}, \nDate: {l.Date}, \nShift: {l.ShiftName}, \nTime In: {l.TimeIn:HH\\:mm}, \nTime Out: {(l.TimeOut != DateTime.MinValue ? (l.TimeOut) : "Ongoing")}, \nWorking Hours: {l.WorkingHours:hh\\:mm\\:ss}, \nLate: {l.LateHours:hh\\:mm\\:ss}, \nOT: {l.OvertimeHours:hh\\:mm\\:ss}\n");
        }
        Console.WriteLine("---------------------\n");
    }
    static void ViewLogs()
    {
        var timeLogs = appService.GetEmployeeLogs(currentEmployeeID);
        Console.WriteLine("\n-----TIME LOGS-----");
        if (!timeLogs.Any())
        {
            Console.WriteLine("No logs to display.\n");
            return;
        }

        foreach (var l in timeLogs)
        {
            Console.WriteLine($"Employee ID: {l.EmployeeID}, \nDate: {l.Date}, \nShift: {l.ShiftName}, \nTime In: {l.TimeIn:HH\\:mm}, \nTime Out: {(l.TimeOut != DateTime.MinValue ? (l.TimeOut) : "Ongoing")}, \nWorking Hours: {l.WorkingHours:hh\\:mm\\:ss}, \nLate: {l.LateHours:hh\\:mm\\:ss}, \nOT: {l.OvertimeHours:hh\\:mm\\:ss}\n");
        }
        Console.WriteLine("---------------------\n");
    }
}