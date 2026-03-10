using System;
using System.ComponentModel;
using System.Data;
using System.Numerics;

internal class Program
{

    static DateTime[] DefaultShiftStart = new DateTime[3];
    static DateTime[] DefaultShiftEnd = new DateTime[3];
    static DateOnly ShiftToday = DateOnly.FromDateTime(DateTime.Now);
    static List <string> TimeLogs = new List <string>();
    static string log;
    static int EmployeeShift;
    static int TimeCheckSelect;

    static DateTime InputTimeIn;
    static DateTime InputTimeOut;

    static void Main(string[] args)
    {
        Console.WriteLine("TIME KEEPING SYSTEM");
        Console.WriteLine($"Date: {ShiftToday}\nWelcome User, ");
        PopulateDefaultSchedules();

        TimeCheckSelect = TimeKeepingEntry();

        UserChoiceChecker();

    }    

    static void UserChoiceChecker()
    {
        while (TimeCheckSelect == 1 || TimeCheckSelect == 2 || TimeCheckSelect == 3)
            {


                if (TimeCheckSelect == 1)
                {
                    ShiftChoicePrompt();
                    TimeInPrompt();
                }
                else if (TimeCheckSelect == 2)
                {
                    ShiftChoicePrompt();
                    TimeOutPrompt();
                }


                TimeCheckSelect = TimeKeepingEntry();
            }

    }
    static void ShiftChoicePrompt()
    {
        Console.WriteLine($"Select Your Assigned Shift Schedule (1-3):\n" +
                $"1. Morning: {DefaultShiftStart[0]} - {DefaultShiftEnd[0]}\n" +
                $"2. Afternoon: {DefaultShiftStart[1]} - {DefaultShiftEnd[1]}\n" +
                $"3. Night: {DefaultShiftStart[2]} - {DefaultShiftEnd[2]}");

        EmployeeShift = Convert.ToInt32(Console.ReadLine()) - 1;
    }
    static void TimeOutPrompt()
    {
        Console.Write("Set Time Out (yyyy-MM-dd HH:mm): ");
        InputTimeOut = DateTime.Parse(Console.ReadLine());
        bool isOverTime = InputTimeOut > DefaultShiftEnd[EmployeeShift];
        if (isOverTime)
        {
            OvertimeCalc();
        }
        else
        {
            UnderTimeCalc();
        }
    }
    static void TimeInPrompt()
    {
        Console.Write("Set Time In (yyyy-MM-dd HH:mm): ");
        InputTimeIn = DateTime.Parse(Console.ReadLine());
        bool islate = InputTimeIn > DefaultShiftStart[EmployeeShift];
        if (islate)
        {
            LateCalc();
        }
        else
        {
            EarlyCalc();
        }
    }
    static void UnderTimeCalc()
    {
        TimeSpan WorkingTime = InputTimeOut - DefaultShiftStart[EmployeeShift];
        TimeSpan UnderTime = DefaultShiftEnd[EmployeeShift] - InputTimeOut;
        log = ($"You worked for {WorkingTime} and you have {UnderTime} Undertime");
        InputLogger();
    }
    static void OvertimeCalc()
    {
        TimeSpan WorkingTime = InputTimeOut - DefaultShiftStart[EmployeeShift];
        TimeSpan Overtime = InputTimeOut - DefaultShiftEnd[EmployeeShift];
        log = ($"You worked for {WorkingTime} and you have {Overtime} Overtime.");
        InputLogger();
    }
    static void EarlyCalc()
    {
        TimeSpan EarlyHours = DefaultShiftStart[EmployeeShift] - InputTimeIn;
        log = ($"You are {EarlyHours} Early.");
        InputLogger();
    }
    static void LateCalc()
    {
        TimeSpan LateHours = InputTimeIn - DefaultShiftStart[EmployeeShift];
        log = ($"You are {LateHours} Late.");
        InputLogger();
    }
    static void InputLogger()
    {
        TimeLogs.Add(log);
        Console.WriteLine(log);
    }
    static void PopulateDefaultSchedules()
    {
        DefaultShiftStart[0] = ShiftToday.ToDateTime(new TimeOnly(6,0,0));
        DefaultShiftStart[1] = DefaultShiftStart[0].AddHours(8);
        DefaultShiftStart[2] = DefaultShiftStart[1].AddHours(8);

        DefaultShiftEnd[0] = DefaultShiftStart[0].AddHours(8);
        DefaultShiftEnd[1] = DefaultShiftStart[1].AddHours(8);
        DefaultShiftEnd[2] = DefaultShiftStart[2].AddHours(8);
    }
    static void PrintLogs()
    {
        foreach (var logs in TimeLogs)
        {
            Console.WriteLine(logs);
        }
        

    }
    static int TimeKeepingEntry()
    {
        Console.WriteLine("Do you want to\n1. Time In?\n2. Time Out?\n3. View Logs\n4. Exit?");
        int TimeCheckSelect = Convert.ToInt32(Console.ReadLine());
        switch (TimeCheckSelect)
        {
            case 1: 
                Console.WriteLine("You have selected Time In.");
                break;
            case 2:
                Console.WriteLine("You have selected Time Out.");
                break;
            case 3:
                Console.WriteLine("You Selected View Logs:");
                PrintLogs();
                break;
            case 4:
                Console.WriteLine("Exiting the program.");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid Selection.");
                Environment.Exit(0);
                break;
        }
        return TimeCheckSelect;
    }

}