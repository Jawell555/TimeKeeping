using System;
using System.ComponentModel;
using System.Data;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;
using TimeKeepingAppService;
using TimeKeepingModels;

internal class Program
{
    static EmployeeAppService appService = new EmployeeAppService();
    static DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
    static int currentEmployeeID;


    static void Main(string[] args)
    {
        MainDisplay();
    }

    static void TimeInOutDisplay()
    {
        bool isAdmin = appService.IsAdmin(currentEmployeeID);
        Console.WriteLine($"\nWelcome, Employee {currentEmployeeID}");
        bool loop = true;
        while (loop == true)
        {
            Console.WriteLine("\nDo you want to:\n" +
                "1. Time In\n" +
                "2. Time Out" +
                "\n3. View Logs");

            if (isAdmin)
            {
                Console.WriteLine("4. View All Logs" +
                    "\n5. View Time Logs By Date" +
                    "\n6. View Latest Logs" +
                    "\n7. View Latest Employee Log" +
                    "\n8. View Shift Schedule" +
                    "\n9. Add Shift Schedule" +
                    "\n10. Update Shift Schedule" +
                    "\n11. Delete Shift Schedule");
            }

            Console.WriteLine("12. Back");
            Console.WriteLine("13. Exit");
            Console.Write("Select Option(1-13): ");

            int choice = IsValidChoice();
            ChoiceSwitch(choice, isAdmin);
            loop = choiceValidation(choice, isAdmin);
        }
    }
    static bool choiceValidation(int choice, bool isAdmin)
    {
        return choice switch
        {
            1 => true,
            2 => true,
            3 => true,
            4 => isAdmin,
            5 => isAdmin,
            6 => isAdmin,
            7 => isAdmin,
            8 => isAdmin,
            9 => isAdmin,
            10 => isAdmin,
            11 => isAdmin,
            12 => false,
            13 => false,
        };

    }
    static int IsValidChoice()
    {
        if (!int.TryParse(Console.ReadLine(), out int inputNumber))
        {
            Console.WriteLine("Invalid format. Please restart and enter a valid number.");
            return -1;
        }
        return inputNumber;
    }
    static TimeOnly GetValidTime(string prompt)
    {
        string[] formats = {
        "hh\\:mm tt",      // 09:30 AM
        "h\\:mm tt",       // 9:30 AM
        "HH\\:mm",         // 09:30 (24-hour)
        "H\\:mm",          // 9:30 (24-hour)
        "hh\\:mm\\:ss tt", // 09:30:45 AM
        "h\\:mm\\:ss tt",  // 9:30:45 AM
        "HH\\:mm\\:ss",    // 09:30:45 (24-hour)
        "H\\:mm\\:ss"      // 9:30:45 (24-hour)
    };

        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input cannot be empty. Try again.\n");
                continue;
            }

            if (TimeOnly.TryParseExact(input, formats, null, System.Globalization.DateTimeStyles.None, out TimeOnly result))
            {
                return result;
            }

            Console.WriteLine("Invalid format. Use hh:mm AM/PM or HH:mm (e.g., 09:30 AM, 09:30 PM, or 21:30)\n");
        }
    }
    static DateOnly GetValidDate(string prompt)
    {
        string[] formats = {
        "MM/dd/yyyy",      // 12/31/2024
        "M/d/yyyy",        // 1/5/2024
        "MM-dd-yyyy",      // 12-31-2024
        "M-d-yyyy",        // 1-5-2024
        "yyyy/MM/dd",      // 2024/12/31
        "yyyy-MM-dd"       // 2024-12-31
    };
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input cannot be empty. Try again.\n");
                continue;
            }
            if (DateOnly.TryParseExact(input, formats, null, System.Globalization.DateTimeStyles.None, out DateOnly result))
            {
                return result;
            }
            Console.WriteLine("Invalid format. Use MM-dd-yyyy (e.g., 12-31-2024)\n");
        }
    }
    static int EmployeeIdValidation()
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
    static void MainDisplay()
    {
        while (true)
        {
            Console.WriteLine("----- TIME KEEPING SYSTEM -----");
            Console.WriteLine($"{dateToday}");
            Console.Write("Enter EmployeeID: ");
            currentEmployeeID = EmployeeIdValidation();
            if (currentEmployeeID == -1)
            {
                continue;
            }
            TimeInOutDisplay();
        }
    }
    static void TimeIn(int employeeID, DateTime timeInTime)
    {
        if (appService.AlreadyTimedIn(employeeID))
        {
            Console.WriteLine("\nYou already timed in.");
            return;

        }
        Employee employee = appService.GetEmployee(employeeID);
        ShiftSchedule shift = appService.GetEmployeeSchedule(employee.ShiftID);
        dateToday = DateOnly.FromDateTime(DateTime.Now);
        DateTime start = dateToday.ToDateTime(shift.ShiftStartTime);
        TimeSpan late = appService.calcLate(start, timeInTime);

        TimeLogs newLog = new TimeLogs { EmployeeID = employeeID, ShiftName = shift.ShiftName, Date = DateOnly.FromDateTime(timeInTime), TimeIn = timeInTime, LateHours = late };

        appService.AddTimeLog(newLog);
        Console.WriteLine($"\nEmployee {employeeID}, is {shift.ShiftName} shift. Timed in at {timeInTime}. Late: {late:hh\\:mm\\:ss}\n");
    }
    static void TimeOut(int employeeID, DateTime timeOutTime)
    {

        TimeLogs log = appService.GetLastTimeIn(employeeID);
        if (log == null)
        {
            Console.WriteLine("\nYou must time in first.");
            return;
        }

        Employee employee = appService.GetEmployee(employeeID);
        ShiftSchedule shift = appService.GetEmployeeSchedule(employee.ShiftID);
        dateToday = DateOnly.FromDateTime(DateTime.Now);
        DateTime end = dateToday.ToDateTime(shift.ShiftEndTime);
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
            Console.WriteLine($"Employee ID: {l.EmployeeID}, \nDate: {l.Date}, \nShift: {l.ShiftName}, \nTime In: {l.TimeIn:HH\\:mm}, \nTime Out: {(l.TimeOut != (DateTime?)null ? (l.TimeOut) : "Ongoing")}, \nWorking Hours: {l.WorkingHours:hh\\:mm\\:ss}, \nLate: {l.LateHours:hh\\:mm\\:ss}, \nOT: {l.OvertimeHours:hh\\:mm\\:ss}\n");
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
            Console.WriteLine($"Employee ID: {l.EmployeeID}, \nDate: {l.Date}, \nShift: {l.ShiftName}, \nTime In: {l.TimeIn:HH\\:mm}, \nTime Out: {(l.TimeOut != (DateTime?)null ? (l.TimeOut) : "Ongoing")}, \nWorking Hours: {l.WorkingHours:hh\\:mm\\:ss}, \nLate: {l.LateHours:hh\\:mm\\:ss}, \nOT: {l.OvertimeHours:hh\\:mm\\:ss}\n");
        }
        Console.WriteLine("---------------------\n");
    }
    static void GetLatestLogs()
    {
        var timeLogs = appService.GetAllLogs();
        Console.WriteLine("\n-----LATEST TIME LOGS-----");
        if (!timeLogs.Any())
        {
            Console.WriteLine("No logs to display.\n");
            return;
        }
        var latestLogs = appService.GetLatestEmployeeLogs();
        foreach (var l in latestLogs)
        {
            Console.WriteLine($"Employee ID: {l.EmployeeID}, \nDate: {l.Date}, \nShift: {l.ShiftName}, \nTime In: {l.TimeIn:HH\\:mm}, \nTime Out: {(l.TimeOut != (DateTime?)null ? (l.TimeOut) : "Ongoing")}, \nWorking Hours: {l.WorkingHours:hh\\:mm\\:ss}, \nLate: {l.LateHours:hh\\:mm\\:ss}, \nOT: {l.OvertimeHours:hh\\:mm\\:ss}\n");
        }
        Console.WriteLine("---------------------\n");
    }
    static void AddShiftSchedule()
    {
        Console.WriteLine("\n-----ADD SHIFT SCHEDULE-----");
        Console.Write("Enter Shift Name: ");
        string shiftName = Console.ReadLine();
        TimeOnly shiftStartTime = GetValidTime("Enter Shift Start Time (hh:mm AM/PM or HH:mm): ");
        TimeOnly shiftEndTime = GetValidTime("Enter Shift End Time (hh:mm AM/PM or HH:mm): ");
        int newShiftID = appService.GenerateShiftID();
        ShiftSchedule newShift = new ShiftSchedule { ShiftID = newShiftID, ShiftName = shiftName, ShiftStartTime = shiftStartTime, ShiftEndTime = shiftEndTime };
        appService.AddShiftSchedule(newShift);
        Console.WriteLine($"\nShift '{shiftName}' added successfully.\n");
    }
    static void UpdateShiftSchedule()
    {
        while (true)
        {
            Console.WriteLine("\n-----UPDATE SHIFT SCHEDULE-----");
            PrintAllSchedules();
            Console.Write("Enter Shift ID to Update: ");
            int shiftID = IsValidChoice();
            if (!appService.ShiftExists(shiftID))
            {
                Console.WriteLine("Shift not found. Try Again.\n");
                continue;
            }
            ShiftSchedule shift = appService.GetEmployeeSchedule(shiftID);
            Console.Write("Enter New Shift Name: ");
            string shiftName = Console.ReadLine();
            TimeOnly shiftStartTime = GetValidTime("Enter New Shift Start Time (HH:mm): ");
            TimeOnly shiftEndTime = GetValidTime("Enter New Shift End Time (HH:mm): ");
            shift.ShiftName = shiftName;
            shift.ShiftStartTime = shiftStartTime;
            shift.ShiftEndTime = shiftEndTime;
            appService.UpdateShiftSchedule(shift);
            Console.WriteLine($"\nShift '{shiftName}' updated successfully.\n");
            return;
        }

    }
    static void DeleteShiftSchedule()
    {

        while (true)
        {
            Console.WriteLine("\n-----DELETE SHIFT SCHEDULE-----");
            PrintAllSchedules();
            Console.Write("Enter Shift ID to Delete: ");
            int shiftID = IsValidChoice();
            if (!appService.ShiftExists(shiftID))
            {
                Console.WriteLine("Shift not found. Try Again.\n");
                continue;
            }
            ShiftSchedule? shift = appService.GetEmployeeSchedule(shiftID);
            appService.DeleteShiftSchedule(shiftID);
            Console.WriteLine($"\nShift '{shift.ShiftName}' deleted successfully.\n");
            return;
        }
    }
    static void PrintAllSchedules()
    {
        var AllShifts = appService.GetAllShiftSchedules();
        if (!AllShifts.Any())
        {
            Console.WriteLine("No shifts available.\n");
            return;
        }
        else
        {
            foreach (var s in AllShifts)
            {
                Console.WriteLine($"Shift ID: {s.ShiftID}, Shift Name: {s.ShiftName}, Start Time: {s.ShiftStartTime:hh\\:mm tt}, End Time: {s.ShiftEndTime:hh\\:mm tt}");
            }
        }
    }
    static void GetLatestEmployeeLog()
    {
        while (true)
        {
            Console.WriteLine("\n-----VIEW LATEST EMPLOYEE LOG-----");
            Console.Write("Enter Employee ID: ");
            int employeeID = IsValidChoice();
            if (!appService.EmployeeExists(employeeID))
            {
                Console.WriteLine("Employee not found. Try Again.\n");
                continue;
            }
            DisplayLatestEmployeeLogByID(employeeID);
            return;
        }
    }
    static void DisplayLatestEmployeeLogByID(int employeeID)
    {
        var log = appService.GetLatestEmployeeLogByID(employeeID);
        if (log == null)
        {
            Console.WriteLine("No logs to display.\n");
            return;
        }
        Console.WriteLine($"Employee ID: {log.EmployeeID}, \nDate: {log.Date}, \nShift: {log.ShiftName}, \nTime In: {log.TimeIn:HH\\:mm}, \nTime Out: {(log.TimeOut != (DateTime?)null ? (log.TimeOut) : "Ongoing")}, \nWorking Hours: {log.WorkingHours:hh\\:mm\\:ss}, \nLate: {log.LateHours:hh\\:mm\\:ss}, \nOT: {log.OvertimeHours:hh\\:mm\\:ss}\n");
    }
    static void ViewTimeLogsByDate()
    {
            Console.WriteLine("\n-----VIEW TIME LOGS BY DATE-----");
            DateOnly date = GetValidDate("Enter Date to View Logs (MM-dd-yyyy): ");
            var logsByDate = appService.GetTimeLogsByDate(date);
            if (!logsByDate.Any())
            {
                Console.WriteLine("No logs to display for this date.\n");
                return;
            }
            foreach (var l in logsByDate)
            {
                Console.WriteLine($"Employee ID: {l.EmployeeID}, \nDate: {l.Date}, \nShift: {l.ShiftName}, \nTime In: {l.TimeIn:HH\\:mm}, \nTime Out: {(l.TimeOut != (DateTime?)null ? (l.TimeOut) : "Ongoing")}, \nWorking Hours: {l.WorkingHours:hh\\:mm\\:ss}, \nLate: {l.LateHours:hh\\:mm\\:ss}, \nOT: {l.OvertimeHours:hh\\:mm\\:ss}\n");
            }
            Console.WriteLine("---------------------\n");
            return;
        
    }
    static void ChoiceSwitch(int choice, bool isAdmin)
    {
        switch (choice)
        {
            case 13:
                Console.WriteLine("Exiting the system. Goodbye!");
                Environment.Exit(0);
                break;
            case 12:
                Console.WriteLine("Successfully Returned to Main Menu.");
                break;
            case 11:
                if (isAdmin)
                {
                    DeleteShiftSchedule();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;
            case 10:
                if (isAdmin)
                {
                    UpdateShiftSchedule();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;
            case 9:
                if (isAdmin)
                {
                    AddShiftSchedule();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;
            case 8:
                if (isAdmin)
                {
                    PrintAllSchedules();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;

            case 7:
                if (isAdmin)
                {
                    GetLatestEmployeeLog();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;

            case 6:
                if (isAdmin)
                {
                    GetLatestLogs();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;
            case 5:
                if (isAdmin)
                {
                    ViewTimeLogsByDate();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
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
    }
}