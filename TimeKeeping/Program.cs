using System;
using System.ComponentModel;
using System.Data;
using System.Numerics;
using TimeKeepingAppService;

internal class Program
{
    static EmployeeAppService appService = new EmployeeAppService();
    static DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);
    static int currentEmployeeID;
    //static DateTime[] DefaultShiftStart = new DateTime[3];
    //static DateTime[] DefaultShiftEnd = new DateTime[3];
    //static DateOnly ShiftToday = DateOnly.FromDateTime(DateTime.Now);
    //static List <string> TimeLogs = new List <string>();
    //static string log;
    //static int EmployeeShift;
    //static int TimeCheckSelect;
    //static bool isTimedIn = false;
    //static int EmployeeID;

    //static DateTime InputTimeIn;
    //static DateTime InputTimeOut;


    static void Main(string[] args)
    {
        mainDisplay();
    }
    static void TimeInOutDisplay()
    {
        bool isAdmin = appService.IsAdmin(currentEmployeeID);
        Console.WriteLine($"Welcome, Employee {currentEmployeeID}");
        Console.WriteLine("\nDo you want to:\n" +
            "1. Time In\n" +
            "2. Time Out");

        if (isAdmin)
        {
            Console.Write("3. View Logs\n");
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
                    appService.ViewLogs();
                }
                else
                {
                    Console.WriteLine("Invalid Selection. Try Again.");
                }
                break;
            case 1:
                appService.TimeIn(currentEmployeeID, DateTime.Now);
                break;
            case 2:
                appService.TimeOut(currentEmployeeID, DateTime.Now);
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
            Console.WriteLine("Enter EmployeeID: ");
            currentEmployeeID = employeeIdValidation();
            if (currentEmployeeID==-1)
            {
                continue;
            }
            TimeInOutDisplay();
        }
    }
    //static void Main(string[] args)
    //{
    //    //Console.WriteLine("TIME KEEPING SYSTEM");
    //    //Console.WriteLine($"Date: {ShiftToday}\nWelcome, ");
    //    //Console.WriteLine("Enter Employee ID: ");
    //    //EmployeeID = Convert.ToInt32(Console.ReadLine());
    //    //PopulateDefaultSchedules();

    //    //TimeCheckSelect = TimeKeepingEntry();
    //    //UserChoiceChecker();
    //}


    //static bool TimeInPass()
    //{
    //    if (isTimedIn)
    //    {
    //        Console.WriteLine("You are already timed in.");
    //        return true;
    //    }
    //    return false;
    //}
    //static bool TimeOutPass()
    //{
    //    if (!isTimedIn)
    //    {
    //        Console.WriteLine("You must time in first.");
    //        return true;
    //    }
    //    return false;
    //}
    //static void ShiftValidation(int EmployeeShift)
    //{
    //    if (!(EmployeeShift <= 2 && EmployeeShift >= 0))
    //    {
    //        Console.WriteLine("Invalid Selection. Try Again.");
    //        ShiftChoicePrompt();
    //    }
    //}
    //static void UserChoiceChecker()
    //{
    //    while (TimeCheckSelect == 1 || TimeCheckSelect == 2 || TimeCheckSelect == 3)
    //        {
    //            if (TimeCheckSelect == 1)
    //            {
    //                if(TimeInPass())
    //                {
    //                    TimeCheckSelect = TimeKeepingEntry();
    //                    continue;
    //                }
    //            ShiftChoicePrompt();
    //                TimeInPrompt();
                    
    //            }
    //            else if (TimeCheckSelect == 2)
    //            {
    //                if(TimeOutPass())
    //                {
    //                    TimeCheckSelect = TimeKeepingEntry();
    //                    continue;
    //            }
    //            ShiftChoicePrompt();
    //                TimeOutPrompt();
    //            }
    //            TimeCheckSelect = TimeKeepingEntry();
    //        }

    //}
    //static void ShiftChoicePrompt()
    //{
    //    Console.WriteLine($"Select Your Assigned Shift Schedule (1-3):\n" +
    //            $"1. Morning: {DefaultShiftStart[0]} - {DefaultShiftEnd[0]}\n" +
    //            $"2. Afternoon: {DefaultShiftStart[1]} - {DefaultShiftEnd[1]}\n" +
    //            $"3. Night: {DefaultShiftStart[2]} - {DefaultShiftEnd[2]}");

    //    EmployeeShift = Convert.ToInt32(Console.ReadLine()) - 1;
    //    ShiftValidation(EmployeeShift);
    //}
    //static void TimeOutPrompt()
    //{
    //    InputTimeOut = DateTime.Now;
    //    Console.WriteLine($"Timed Out: {InputTimeOut}");
    //    bool isOverTime = InputTimeOut > DefaultShiftEnd[EmployeeShift];
    //    if (isOverTime)
    //    {
    //        OvertimeCalc();
    //        isTimedIn = false;
    //    }
    //    else
    //    {
    //        UnderTimeCalc();
    //        isTimedIn = false;
    //    }
    //}
    //static void TimeInPrompt()
    //{
    //    InputTimeIn = DateTime.Now;
    //    Console.WriteLine($"Timed In: {InputTimeIn}");
    //    bool islate = InputTimeIn > DefaultShiftStart[EmployeeShift];
    //    if (islate)
    //    {
    //        LateCalc();
    //        isTimedIn = true;
    //    }
    //    else
    //    {
    //        EarlyCalc();
    //        isTimedIn = true;
    //    }
    //}
    //static void UnderTimeCalc()
    //{
    //    TimeSpan WorkingTime = InputTimeOut - InputTimeIn;
    //    TimeSpan UnderTime = DefaultShiftEnd[EmployeeShift] - InputTimeOut;
    //    log = ($"Employee {EmployeeID} Time Out: {InputTimeOut} | Working Hours: {WorkingTime}| Undertime: {UnderTime}");
    //    InputLogger();
    //}
    //static void OvertimeCalc()
    //{
    //    TimeSpan WorkingTime = InputTimeOut - InputTimeIn;
    //    TimeSpan Overtime = InputTimeOut - DefaultShiftEnd[EmployeeShift];
    //    log = ($"Employee {EmployeeID} Time Out: {InputTimeOut} | Working Hours: {WorkingTime}| Overtime: {Overtime}");
    //    InputLogger();
    //}
    //static void EarlyCalc()
    //{
    //    TimeSpan EarlyHours = DefaultShiftStart[EmployeeShift] - InputTimeIn;
    //    log = ($"Employee {EmployeeID} Time In: {InputTimeIn} | Early: {EarlyHours}");
    //    InputLogger();
    //}
    //static void LateCalc()
    //{
    //    TimeSpan LateHours = InputTimeIn - DefaultShiftStart[EmployeeShift];
    //    log = ($"Employee {EmployeeID} Time In: {InputTimeIn} | Late: {LateHours}");
    //    InputLogger();
    //}
    //static void InputLogger()
    //{
    //    TimeLogs.Add(log);
    //    Console.WriteLine(log);
    //}
    //static void PopulateDefaultSchedules()
    //{
    //    DefaultShiftStart[0] = ShiftToday.ToDateTime(new TimeOnly(6,0,0));
    //    DefaultShiftStart[1] = DefaultShiftStart[0].AddHours(8);
    //    DefaultShiftStart[2] = DefaultShiftStart[1].AddHours(8);

    //    DefaultShiftEnd[0] = DefaultShiftStart[0].AddHours(8);
    //    DefaultShiftEnd[1] = DefaultShiftStart[1].AddHours(8);
    //    DefaultShiftEnd[2] = DefaultShiftStart[2].AddHours(8);
    //}
    //static void PrintLogs()
    //{
    //    foreach (var logs in TimeLogs)
    //    {
    //        Console.WriteLine(logs);
    //    }
    //}
    //static int TimeKeepingEntry()
    //{
    //    Console.WriteLine("Do you want to\n1. Time In?\n2. Time Out?\n3. View Logs\n4. Exit?");
    //    int TimeCheckSelect = Convert.ToInt32(Console.ReadLine());
    //    switch (TimeCheckSelect)
    //    {
    //        case 1: 
    //            Console.WriteLine("You have selected Time In.");
    //            break;
    //        case 2:
    //            Console.WriteLine("You have selected Time Out.");
    //            break;
    //        case 3:
    //            Console.WriteLine("You Selected View Logs:");
    //            PrintLogs();
    //            break;
    //        case 4:
    //            Console.WriteLine("Exiting the program.");
    //            Environment.Exit(0);
    //            break;
    //        default:
    //            Console.WriteLine("Invalid Selection. Try Again.");
    //            return TimeKeepingEntry();
    //    }
    //    return TimeCheckSelect;
    //}

}