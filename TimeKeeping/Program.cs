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
   
}