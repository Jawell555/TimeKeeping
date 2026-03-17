using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TimeKeepingModels;

namespace TimeKeepingDataService
{
    public class EmployeeShiftingJsonData
    {
        public List<Employee> Employees = new List<Employee>();
        private List<ShiftSchedule> ShiftSchedules = new List<ShiftSchedule>();
        private List<TimeLogs> TimeInOutLogs = new List<TimeLogs>();

        private string _employeeJsonFilePath, _shiftingJsonFilePath, _logsJsonFilePath;
        public EmployeeShiftingJsonData() {
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
        public ShiftSchedule? GetEmployeeShift(Employee employee)
        {
            RetrieveShiftingDataFromJsonFile();
            return ShiftSchedules.FirstOrDefault(s => s.ShiftID == employee.ShiftID);
        }
        public TimeLogs? GetLogByDate(int employeeID, DateTime timeOutTime)
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs.FirstOrDefault(l => l.EmployeeID == employeeID && l.Date == DateOnly.FromDateTime(timeOutTime) && l.TimeOut == DateTime.MinValue);
        }
        public void UpdateTimeLog(TimeLogs log)
        {
            RetrieveTimeLogsFromJsonFile();
            var existingLog = TimeInOutLogs.FirstOrDefault(l => l.EmployeeID == log.EmployeeID && l.Date == log.Date);
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
        public List<TimeLogs> GetAllLogs()
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs;
        }
        public bool AlreadyTimedIn(int employeeID, DateTime timeInTime)
        {
            RetrieveTimeLogsFromJsonFile();
            return TimeInOutLogs.Any(l => l.EmployeeID == employeeID && l.Date == DateOnly.FromDateTime(timeInTime) && l.TimeOut == DateTime.MinValue);
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
            if(ShiftSchedules.Count <= 0)
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                DateTime shiftStart1 = today.ToDateTime(new TimeOnly(6, 0));
                DateTime shiftStart2 = shiftStart1.AddHours(8);
                DateTime shiftStart3 = shiftStart2.AddHours(8);

                ShiftSchedules.Add(new ShiftSchedule { ShiftID = 1, ShiftName = "Morning", ShiftStartTime = shiftStart1, ShiftEndTime = shiftStart1.AddHours(8)});
                ShiftSchedules.Add(new ShiftSchedule { ShiftID = 2, ShiftName = "Afternoon", ShiftStartTime = shiftStart2, ShiftEndTime = shiftStart2.AddHours(8) });
                ShiftSchedules.Add(new ShiftSchedule { ShiftID = 3, ShiftName = "Night", ShiftStartTime = shiftStart1, ShiftEndTime = shiftStart3.AddHours(8) });
                
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
                this.Employees =JsonSerializer.Deserialize<List<Employee>>(jsonFileReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).ToList();
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
                JsonSerializer.Serialize<List<Employee>>(new Utf8JsonWriter(outputStream, new JsonWriterOptions { SkipValidation = true, Indented = true}),Employees);
            }
        }

        private void RetrieveShiftingDataFromJsonFile()
        {
            using (var jsonFileReader = File.OpenText(this._shiftingJsonFilePath))
            {
                this.ShiftSchedules = JsonSerializer.Deserialize<List<ShiftSchedule>>(jsonFileReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).ToList();
            }
        }
    }
}
