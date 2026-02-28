using System;
using System.ComponentModel;
using System.Numerics;

internal class Program
{

    static DateTime[] DefaultShiftStart = new DateTime[3];
    static DateTime[] DefaultShiftEnd = new DateTime[3];
    static DateOnly ShiftToday = DateOnly.FromDateTime(DateTime.Now);
    static List <string> TimeLogs = new List <string>();
    static string log;

    static DateTime InputTimeIn;
    static DateTime InputTimeOut;

    static void Main(string[] args)
    {
        Console.WriteLine("TIME KEEPING SYSTEM");
        Console.WriteLine($"Date: {ShiftToday}\nWelcome User, ");
        PopulateDefaultSchedules();

        int TimeCheckSelect = UserTimeInOut();

        while (TimeCheckSelect == 1 || TimeCheckSelect == 2 || TimeCheckSelect ==3)
        {
            Console.WriteLine($"Select Your Assigned Shift Schedule (1-3):\n" +
                $"1. Morning: {DefaultShiftStart[0]} - {DefaultShiftEnd[0]}\n" +
                $"2. Afternoon: {DefaultShiftStart[1]} - {DefaultShiftEnd[1]}\n" +
                $"3. Night: {DefaultShiftStart[2]} - {DefaultShiftEnd[2]}");

            int EmployeeShift = Convert.ToInt32( Console.ReadLine() )-1;

            if (TimeCheckSelect == 1)
            {
                Console.Write("Set Time In (yyyy-MM-dd HH:mm): ");
                InputTimeIn = DateTime.Parse(Console.ReadLine());
                bool islate = InputTimeIn > DefaultShiftStart[EmployeeShift];
                if (islate)
                {
                    TimeSpan LateHours = InputTimeIn - DefaultShiftStart[EmployeeShift];
                    log = ($"You are {LateHours} Late.");
                    InputLogger();
                }
                else
                {
                    TimeSpan EarlyHours = DefaultShiftStart[EmployeeShift] - InputTimeIn;
                    log = ($"You are {EarlyHours} EarlyHours.");
                    InputLogger();
                }
            }
            else if (TimeCheckSelect == 2)
            {
                Console.Write("Set Time Out (yyyy-MM-dd HH:mm): ");
                InputTimeOut = DateTime.Parse(Console.ReadLine());
                bool isOverTime = InputTimeOut > DefaultShiftEnd[EmployeeShift];

                if (isOverTime)
                {
                    TimeSpan WorkingTime = InputTimeOut - DefaultShiftStart[EmployeeShift];
                    TimeSpan Overtime = InputTimeOut - DefaultShiftEnd[EmployeeShift];
                    log = ($"You worked for {WorkingTime} and you have {Overtime} Overtime.");
                    InputLogger();
                }
                else
                {
                    TimeSpan WorkingTime = InputTimeOut - DefaultShiftStart[EmployeeShift];
                    TimeSpan UnderTime = DefaultShiftEnd[EmployeeShift] - InputTimeOut;
                    log = ($"You worked for {WorkingTime} and you have {UnderTime} Undertime");
                    InputLogger();
                }
            }
            
            
                TimeCheckSelect = UserTimeInOut();
        }
        
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
    static int UserTimeInOut()
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
                foreach (var logs in TimeLogs)
                {
                    Console.WriteLine(logs);
                }
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