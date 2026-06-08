using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TimeKeepingModels;

namespace TimeKeepingManagementDataService
{
    public class EmployeeShiftingJsonData : ITimeKeepingDataService
    {
        public List<Employee> Employees = new List<Employee>();
        private List<ShiftSchedule> ShiftSchedules = new List<ShiftSchedule>();
        private List<TimeLogs> TimeInOutLogs = new List<TimeLogs>();

        private string _employeeJsonFilePath, _shiftingJsonFilePath, _logsJsonFilePath;
        public EmployeeShiftingJsonData()
        {
            _employeeJsonFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}Employee.json";
            _shiftingJsonFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}ShiftSchedule.json";
            _logsJsonFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}TimeLogs.json";

            PopulateJsonFiles();
        }
        public void Add(Employee employee)
        {
            Employees.Add(employee);
            SaveEmployeeDataToJsonFiles();
        }
        public void Add(ShiftSchedule shift)
        {
            ShiftSchedules.Add(shift);
            SaveShiftDataToJsonFiles();
        }
        public void Add(TimeLogs timeLog)
        {
            TimeInOutLogs.Add(timeLog);
            SaveTimeLogsToJsonFile();
        }
        public Employee? GetEmployeeByID(int employeeID)
        {
            RetrieveEmployeeDataFromJsonFile();
            return Employees.FirstOrDefault(e => e.EmployeeID == employeeID);
        }
        public ShiftSchedule? GetEmployeeShift(int shiftID)
        {
            RetrieveShiftingDataFromJsonFile();
            return ShiftSchedules.FirstOrDefault(s => s.ShiftID == shiftID);
        }
        public TimeLogs? GetLastTimeIn(int employeeID)
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs.FirstOrDefault(l => l.EmployeeID == employeeID && l.TimeOut == null);
        }
        public void UpdateTimeLog(TimeLogs log)
        {
            RetrieveTimeLogsFromJsonFile();
            var existingLog = TimeInOutLogs.LastOrDefault(l => l.EmployeeID == log.EmployeeID && l.TimeOut == null);
            if (existingLog != null)
            {
                existingLog.TimeIn = log.TimeIn;
                existingLog.TimeOut = log.TimeOut;
                existingLog.WorkingHours = log.WorkingHours;
                existingLog.LateHours = log.LateHours;
                existingLog.OvertimeHours = log.OvertimeHours;
                existingLog.UndertimeHours = log.UndertimeHours;
                SaveTimeLogsToJsonFile();
            }
        }
        public List<TimeLogs> GetEmployeeLogs(int employeeID)
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs.FindAll(l => l.EmployeeID == employeeID);
        }
        public List<TimeLogs> GetAllLogs()
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs;
        }
        public bool AlreadyTimedIn(int employeeID)
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs.Any(l => l.EmployeeID == employeeID && l.TimeOut == null);
        }
        public bool EmployeeExists(int Employee)
        {
            RetrieveEmployeeDataFromJsonFile();
            return Employees.Any(e => e.EmployeeID == Employee);

        }
        private void PopulateJsonFiles()
        {
            RetrieveEmployeeDataFromJsonFile();
            if (Employees.Count <= 0)
            {
                Employees.Add(new Employee { EmployeeID = 0, ShiftID = 1, IsAdmin = true });
                Employees.Add(new Employee { EmployeeID = 1, ShiftID = 2, IsAdmin = false });
                Employees.Add(new Employee { EmployeeID = 2, ShiftID = 3, IsAdmin = false });
                SaveEmployeeDataToJsonFiles();
            }
            RetrieveShiftingDataFromJsonFile();
            if (ShiftSchedules.Count <= 0)
            {
                TimeOnly shiftStart1 = new TimeOnly(6, 0);
                TimeOnly shiftStart2 = shiftStart1.AddHours(8);
                TimeOnly shiftStart3 = shiftStart2.AddHours(8);

                ShiftSchedules.Add(new ShiftSchedule { ShiftID = 1, ShiftName = "Morning", ShiftStartTime = shiftStart1, ShiftEndTime = shiftStart1.AddHours(8) });
                ShiftSchedules.Add(new ShiftSchedule { ShiftID = 2, ShiftName = "Afternoon", ShiftStartTime = shiftStart2, ShiftEndTime = shiftStart2.AddHours(8) });
                ShiftSchedules.Add(new ShiftSchedule { ShiftID = 3, ShiftName = "Night", ShiftStartTime = shiftStart3, ShiftEndTime = shiftStart3.AddHours(8) });

                SaveShiftDataToJsonFiles();
            }
        }
        private void RetrieveTimeLogsFromJsonFile()
        {
            using (var jsonFileReader = File.OpenText(this._logsJsonFilePath))
            {
                this.TimeInOutLogs = JsonSerializer.Deserialize<List<TimeLogs>>(jsonFileReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).ToList();
            }
        }
        private void SaveTimeLogsToJsonFile()
        {
            using (var outputStream = File.OpenWrite(this._logsJsonFilePath))
            {
                JsonSerializer.Serialize<List<TimeLogs>>(new Utf8JsonWriter(outputStream, new JsonWriterOptions { SkipValidation = true, Indented = true }), TimeInOutLogs);
            }
        }
        private void RetrieveEmployeeDataFromJsonFile()
        {
            using (var jsonFileReader = File.OpenText(this._employeeJsonFilePath))
            {
                this.Employees = JsonSerializer.Deserialize<List<Employee>>(jsonFileReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).ToList();
            }
        }
        private void SaveShiftDataToJsonFiles()
        {
            using (var outputStream = File.OpenWrite(this._shiftingJsonFilePath))
            {
                JsonSerializer.Serialize<List<ShiftSchedule>>(new Utf8JsonWriter(outputStream, new JsonWriterOptions { SkipValidation = true, Indented = true }), ShiftSchedules);
            }
        }
        private void SaveEmployeeDataToJsonFiles()
        {
            using (var outputStream = File.OpenWrite(this._employeeJsonFilePath))
            {
                JsonSerializer.Serialize<List<Employee>>(new Utf8JsonWriter(outputStream, new JsonWriterOptions { SkipValidation = true, Indented = true }), Employees);
            }
        }
        private void RetrieveShiftingDataFromJsonFile()
        {
            using (var jsonFileReader = File.OpenText(this._shiftingJsonFilePath))
            {
                this.ShiftSchedules = JsonSerializer.Deserialize<List<ShiftSchedule>>(jsonFileReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).ToList();
            }
        }

        public List<Employee> GetEmployees()
        {
            RetrieveEmployeeDataFromJsonFile();
            return Employees;
        }

        public List<ShiftSchedule> GetShifts()
        {
            RetrieveShiftingDataFromJsonFile();
            return ShiftSchedules;
        }

        public List<TimeLogs> GetLatestEmployeeLogs()
        {
            RetrieveTimeLogsFromJsonFile();

            return TimeInOutLogs
                .GroupBy(l => l.EmployeeID)
                .Select(g => g.OrderByDescending(l => l.Date)
                               .ThenByDescending(l => l.TimeIn)
                               .FirstOrDefault())
                .Where(l => l != null)
                .ToList();
        }

        public TimeLogs? GetLatestEmployeeLogByID(int employeeID)
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs
                .Where(l => l.EmployeeID == employeeID)
                .OrderByDescending(l => l.Date)
                .ThenByDescending(l => l.TimeIn)
                .FirstOrDefault();
        }
        public void AddShiftSchedule(ShiftSchedule shift)
        {
            ShiftSchedules.Add(shift);
            SaveShiftDataToJsonFiles();
        }
        public int GenerateShiftID()
        {
            int newShiftID = ShiftSchedules.Count > 0 ? ShiftSchedules.Max(s => s.ShiftID) + 1 : 1;
            return newShiftID;
        }
        public void UpdateShiftSchedule(ShiftSchedule shift)
        {
            RetrieveShiftingDataFromJsonFile();
            var existingShift = ShiftSchedules.FirstOrDefault(s => s.ShiftID == shift.ShiftID);
            if (existingShift != null)
            {
                existingShift.ShiftName = shift.ShiftName;
                existingShift.ShiftStartTime = shift.ShiftStartTime;
                existingShift.ShiftEndTime = shift.ShiftEndTime;
                SaveShiftDataToJsonFiles();
            }

        }
        public void DeleteShiftSchedule(int shiftID)
        {
            RetrieveShiftingDataFromJsonFile();
            var shiftToDelete = ShiftSchedules.FirstOrDefault(s => s.ShiftID == shiftID);
            if (shiftToDelete != null)
            {
                ShiftSchedules.Remove(shiftToDelete);
                SaveShiftDataToJsonFiles();
            }
        }
    }
}
