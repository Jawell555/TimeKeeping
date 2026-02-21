using System;
using System.ComponentModel;
using System.Numerics;

internal class Program
{
    static void Main(string[] args)
    {
        TimeOnly[] ShiftingSchedule = new TimeOnly[3];
        string[] Shifts = {"Morning", "Afternoon","Night"};
        List<string> ShiftLogs = new List<string>();

        //DefaultShiftingSchedule
        ShiftingSchedule[0] = new TimeOnly(6,00);
        ShiftingSchedule[1] = new TimeOnly(14,00);
        ShiftingSchedule[2] = new TimeOnly(22, 00);
        
        Console.Write("Enter Time In (HH:mm): ");
        TimeOnly TimeInInput = TimeOnly.Parse(Console.ReadLine());
        Console.WriteLine("Enter your Shift Schedule:\n" +
            "[1] Morning Shift:     6AM - 2PM\n" +
            "[2] Afternoon Shift:   2PM - 10PM\n" +
            "[3] Night Shift:       10PM - 6AM");
        int ShiftInput = Convert.ToInt32(Console.ReadLine())-1;


        Console.WriteLine(TimeInInput);
        bool isLate = EmployeeLate(ShiftInput, TimeInInput, ShiftingSchedule);

        

        ShiftLogs.Add($"Time In: {TimeInInput}, Shift: {Shifts[ShiftInput]}, isLate?: {isLate}");
        foreach (var log in ShiftLogs)
        {
            Console.WriteLine(log);
        }
    }
    static bool EmployeeLate(int shift, TimeOnly time, TimeOnly[] times) {
        switch (shift)
        {
            case 0:
                if (time <= times[shift])
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case 1:
                if (time <= times[shift])
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case 2:
                if (!time.IsBetween(times[2], times[0]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            default:
                return false;
                break;
        }
    }
}