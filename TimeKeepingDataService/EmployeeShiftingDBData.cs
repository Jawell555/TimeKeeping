using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TimeKeepingModels;

namespace TimeKeepingManagementDataService
{
    public class EmployeeShiftingDBData : ITimeKeepingDataService
    {
        private string _connectionString = "Server=localhost\\SQLEXPRESS;Database=EmployeesManagement;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        private SqlConnection _connection;

        public EmployeeShiftingDBData()
        {
            _connection = new SqlConnection(_connectionString);

            AddSeeds();
        }

        private void AddSeeds()
        {
            var existingShifts = GetShifts();
            if (existingShifts.Count == 0)
            {
                TimeOnly shiftStart1 = new TimeOnly(6, 0);
                TimeOnly shiftStart2 = shiftStart1.AddHours(8);
                TimeOnly shiftStart3 = shiftStart2.AddHours(8);

                ShiftSchedule morningShift = new ShiftSchedule { ShiftID = 1, ShiftName = "Morning", ShiftStartTime = shiftStart1, ShiftEndTime = shiftStart1.AddHours(8) };
                ShiftSchedule afternoonShift = new ShiftSchedule { ShiftID = 2, ShiftName = "Afternoon", ShiftStartTime = shiftStart2, ShiftEndTime = shiftStart2.AddHours(8) };
                ShiftSchedule nightShift = new ShiftSchedule { ShiftID = 3, ShiftName = "Night", ShiftStartTime = shiftStart3, ShiftEndTime = shiftStart3.AddHours(8) };

                Add(morningShift);
                Add(afternoonShift);
                Add(nightShift);
            }
            var existingEmployees = GetEmployees();
            if (existingEmployees.Count == 0)
            {
                Employee admin = new Employee { EmployeeID = 0, ShiftID = 1, IsAdmin = true };
                Employee employee1 = new Employee { EmployeeID = 1, ShiftID = 2, IsAdmin = false };
                Employee employee2 = new Employee { EmployeeID = 2, ShiftID = 3, IsAdmin = false };
                Add(admin);
                Add(employee1);
                Add(employee2);
            }

        }
        public List<ShiftSchedule> GetShifts()
        {
            var selectStatement = "SELECT * FROM dbo.ShiftSchedules";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            _connection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            var shifts = new List<ShiftSchedule>();
            while (reader.Read())
            {
                ShiftSchedule shift = new ShiftSchedule
                {
                    ShiftID = reader.GetInt32(0),
                    ShiftName = reader.GetString(1),
                    ShiftStartTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(2)),
                    ShiftEndTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(3))
                };
                shifts.Add(shift);
            }
            _connection.Close();
            return shifts;
        }

        public List<Employee> GetEmployees()
        {
            var selectStatement = "SELECT * FROM dbo.Employees";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            _connection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            var employees = new List<Employee>();
            while (reader.Read())
            {
                Employee employee = new Employee
                {
                    EmployeeID = reader.GetInt32(0),
                    ShiftID = reader.GetInt32(1),
                    IsAdmin = reader.GetBoolean(2)
                };
                employees.Add(employee);
            }
            _connection.Close();
            return employees;
        }

        public void Add(Employee employee)
        {
            var insertStatement = "INSERT INTO dbo.Employees (EmployeeID, ShiftID, IsAdmin) VALUES (@EmployeeID, @ShiftID, @IsAdmin)";

            SqlCommand insertCommand = new SqlCommand(insertStatement, _connection);

            insertCommand.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
            insertCommand.Parameters.AddWithValue("@ShiftID", employee.ShiftID);
            insertCommand.Parameters.AddWithValue("@IsAdmin", employee.IsAdmin);
            _connection.Open();
            insertCommand.ExecuteNonQuery();

            _connection.Close();
        }

        public void Add(ShiftSchedule shift)
        {
            var insertStatement = "INSERT INTO dbo.ShiftSchedules (ShiftID, ShiftName, ShiftStartTime, ShiftEndTime) VALUES (@ShiftID, @ShiftName, @ShiftStartTime, @ShiftEndTime)";

            SqlCommand insertCommand = new SqlCommand(insertStatement, _connection);

            insertCommand.Parameters.AddWithValue("@ShiftID", shift.ShiftID);
            insertCommand.Parameters.AddWithValue("@ShiftName", shift.ShiftName);
            insertCommand.Parameters.AddWithValue("@ShiftStartTime", shift.ShiftStartTime);
            insertCommand.Parameters.AddWithValue("@ShiftEndTime", shift.ShiftEndTime);
            _connection.Open();
            insertCommand.ExecuteNonQuery();

            _connection.Close();
        }

        public void Add(TimeLogs timeLog)
        {
            var insertStatement = "INSERT INTO dbo.TimeLogs (EmployeeID, ShiftName, [Date], TimeIn, TimeOut, WorkingHours, LateHours, OvertimeHours, UndertimeHours) VALUES (@EmployeeID, @ShiftName, @Date, @TimeIn, @TimeOut, @WorkingHours, @LateHours, @OvertimeHours, @UndertimeHours)";
            SqlCommand insertCommand = new SqlCommand(insertStatement, _connection);
            insertCommand.Parameters.AddWithValue("@EmployeeID", timeLog.EmployeeID);
            insertCommand.Parameters.AddWithValue("@ShiftName", timeLog.ShiftName);
            insertCommand.Parameters.AddWithValue("@Date", timeLog.Date);
            insertCommand.Parameters.AddWithValue("@TimeIn", timeLog.TimeIn);
            insertCommand.Parameters.AddWithValue("@TimeOut", DBNull.Value);
            insertCommand.Parameters.AddWithValue("@WorkingHours", timeLog.WorkingHours);
            insertCommand.Parameters.AddWithValue("@LateHours", timeLog.LateHours);
            insertCommand.Parameters.AddWithValue("@OvertimeHours", timeLog.OvertimeHours);
            insertCommand.Parameters.AddWithValue("@UndertimeHours", timeLog.UndertimeHours);
            _connection.Open();
            insertCommand.ExecuteNonQuery();
            _connection.Close();

        }

        public bool AlreadyTimedIn(int employeeID)
        {
            var selectStatement = "SELECT COUNT(*) FROM dbo.TimeLogs WHERE EmployeeID = @EmployeeID AND TimeOut IS NULL";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            _connection.Open();
            selectCommand.Parameters.AddWithValue("@EmployeeID", employeeID);

            var rows = Convert.ToInt32(selectCommand.ExecuteScalar());

            _connection.Close();
            return rows > 0;
        }


        public bool EmployeeExists(int employeeID)
        {
            var selectStatement = "SELECT COUNT(*) FROM dbo.Employees WHERE EmployeeID = @EmployeeID";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);

            _connection.Open();
            selectCommand.Parameters.AddWithValue("@EmployeeID", employeeID);

            var rows = Convert.ToInt32(selectCommand.ExecuteScalar());

            _connection.Close();
            return rows > 0;
        }
        public bool ShiftExists(int shiftID)
        {
            var selectStatement = "SELECT COUNT(*) FROM dbo.ShiftSchedules WHERE ShiftID = @ShiftID";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);

            _connection.Open();
            selectCommand.Parameters.AddWithValue("@ShiftID", shiftID);
            var rows = Convert.ToInt32(selectCommand.ExecuteScalar());

            _connection.Close();
            return rows > 0;
        }

        public List<TimeLogs> GetAllLogs()
        {
            var selectStatement = "SELECT * FROM dbo.TimeLogs";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            _connection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            var logs = new List<TimeLogs>();
            while (reader.Read())
            {
                TimeLogs log = new TimeLogs
                {
                    EmployeeID = reader.GetInt32(0),
                    ShiftName = reader.GetString(1),
                    Date = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    TimeIn = reader.GetDateTime(3),
                    TimeOut = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                    WorkingHours = reader.GetTimeSpan(5),
                    LateHours = reader.GetTimeSpan(6),
                    OvertimeHours = reader.GetTimeSpan(7),
                    UndertimeHours = reader.GetTimeSpan(8)
                };
                logs.Add(log);
            }
            _connection.Close();
            return logs;
        }

        public Employee? GetEmployeeByID(int employeeID)
        {
            var selectStatement = "SELECT * FROM dbo.Employees WHERE EmployeeID = @EmployeeID";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            selectCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
            _connection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();

            var employee = new Employee();
            while (reader.Read())
            {
                employee.EmployeeID = reader.GetInt32(0);
                employee.ShiftID = reader.GetInt32(1);
                employee.IsAdmin = reader.GetBoolean(2);
            }
            _connection.Close();
            return employee;
        }

        public ShiftSchedule? GetEmployeeShift(int shiftID)
        {
            var SelectStatement = "SELECT * FROM dbo.ShiftSchedules WHERE ShiftID = @shiftID";
            SqlCommand selectCommand = new SqlCommand(SelectStatement, _connection);
            _connection.Open();
            selectCommand.Parameters.AddWithValue("@shiftID", shiftID);
            SqlDataReader reader = selectCommand.ExecuteReader();
            var shift = new ShiftSchedule();
            while (reader.Read())
            {
                shift.ShiftID = reader.GetInt32(0);
                shift.ShiftName = reader.GetString(1);
                shift.ShiftStartTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(2));
                shift.ShiftEndTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(3));
            }
            _connection.Close();
            return shift;
        }

        public TimeLogs? GetLastTimeIn(int employeeID)
        {
            var selectStatement = "SELECT * FROM TimeLogs WHERE EmployeeID = @EmployeeID  AND TimeOut IS NULL";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            selectCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
            _connection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            var log = new TimeLogs();
            if (!reader.Read())
            {
                _connection.Close();
                return null;
            }

            log.EmployeeID = reader.GetInt32(0);
            log.ShiftName = reader.GetString(1);
            log.Date = DateOnly.FromDateTime(reader.GetDateTime(2));
            log.TimeIn = reader.GetDateTime(3);
            log.TimeOut = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
            log.WorkingHours = reader.GetTimeSpan(5);
            log.LateHours = reader.GetTimeSpan(6);
            log.OvertimeHours = reader.GetTimeSpan(7);
            log.UndertimeHours = reader.GetTimeSpan(8);


            _connection.Close();
            return log;

        }

        public void UpdateTimeLog(TimeLogs log)
        {
            var updateStatement = "UPDATE dbo.TimeLogs SET TimeOut = @TimeOut, WorkingHours = @WorkingHours, LateHours = @LateHours, OvertimeHours = @OvertimeHours, UndertimeHours = @UndertimeHours WHERE EmployeeID = @EmployeeID  AND TimeOut IS NULL";
            SqlCommand updateCommand = new SqlCommand(updateStatement, _connection);
            updateCommand.Parameters.AddWithValue("@TimeOut", log.TimeOut);
            updateCommand.Parameters.AddWithValue("@WorkingHours", log.WorkingHours);
            updateCommand.Parameters.AddWithValue("@LateHours", log.LateHours);
            updateCommand.Parameters.AddWithValue("@OvertimeHours", log.OvertimeHours);
            updateCommand.Parameters.AddWithValue("@UndertimeHours", log.UndertimeHours);
            updateCommand.Parameters.AddWithValue("@EmployeeID", log.EmployeeID);
            _connection.Open();
            updateCommand.ExecuteNonQuery();
            _connection.Close();
        }

        public List<TimeLogs> GetEmployeeLogs(int employeeID)
        {
            var selectStatement = "SELECT * FROM dbo.TimeLogs WHERE EmployeeID = @EmployeeID";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            _connection.Open();
            selectCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
            SqlDataReader reader = selectCommand.ExecuteReader();
            var logs = new List<TimeLogs>();
            while (reader.Read())
            {
                TimeLogs log = new TimeLogs
                {
                    EmployeeID = reader.GetInt32(0),
                    ShiftName = reader.GetString(1),
                    Date = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    TimeIn = reader.GetDateTime(3),
                    TimeOut = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                    WorkingHours = reader.GetTimeSpan(5),
                    LateHours = reader.GetTimeSpan(6),
                    OvertimeHours = reader.GetTimeSpan(7),
                    UndertimeHours = reader.GetTimeSpan(8)
                };
                logs.Add(log);
            }
            _connection.Close();
            return logs;
        }

        public List<TimeLogs> GetLatestEmployeeLogs()
        {
            var selectStatement = "SELECT * FROM ( SELECT *, ROW_NUMBER() OVER (PARTITION BY EmployeeID ORDER BY [Date] DESC, TimeIn DESC) AS rn FROM TimeLogs) ranked WHERE rn = 1 ORDER BY EmployeeID;";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            _connection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            var logs = new List<TimeLogs>();
            while (reader.Read())
            {
                TimeLogs log = new TimeLogs
                {
                    EmployeeID = reader.GetInt32(0),
                    ShiftName = reader.GetString(1),
                    Date = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    TimeIn = reader.GetDateTime(3),
                    TimeOut = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                    WorkingHours = reader.GetTimeSpan(5),
                    LateHours = reader.GetTimeSpan(6),
                    OvertimeHours = reader.GetTimeSpan(7),
                    UndertimeHours = reader.GetTimeSpan(8)
                };
                logs.Add(log);
            }
            _connection.Close();
            return logs;
        }

        public TimeLogs? GetLatestEmployeeLogByID(int employeeID)
        {
            var selectStatement = "SELECT TOP 1 * FROM TimeLogs WHERE EmployeeID = @EmployeeID ORDER BY Date DESC, TimeIn DESC;";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            selectCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
            _connection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            var log = new TimeLogs();
            if (!reader.Read())
            {
                _connection.Close();
                return null;
            }

            log.EmployeeID = reader.GetInt32(0);
            log.ShiftName = reader.GetString(1);
            log.Date = DateOnly.FromDateTime(reader.GetDateTime(2));
            log.TimeIn = reader.GetDateTime(3);
            log.TimeOut = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
            log.WorkingHours = reader.GetTimeSpan(5);
            log.LateHours = reader.GetTimeSpan(6);
            log.OvertimeHours = reader.GetTimeSpan(7);
            log.UndertimeHours = reader.GetTimeSpan(8);


            _connection.Close();
            return log;

        }
        public void AddShiftSchedule(ShiftSchedule shift)
        {
            var insertStatement = "INSERT INTO dbo.ShiftSchedules (ShiftID, ShiftName, ShiftStartTime, ShiftEndTime) VALUES (@ShiftID, @ShiftName, @ShiftStartTime, @ShiftEndTime)";
            SqlCommand insertCommand = new SqlCommand(insertStatement, _connection);
            insertCommand.Parameters.AddWithValue("@ShiftID", shift.ShiftID);
            insertCommand.Parameters.AddWithValue("@ShiftName", shift.ShiftName);
            insertCommand.Parameters.AddWithValue("@ShiftStartTime", shift.ShiftStartTime);
            insertCommand.Parameters.AddWithValue("@ShiftEndTime", shift.ShiftEndTime);
            _connection.Open();
            insertCommand.ExecuteNonQuery();
            _connection.Close();
        }
        public int GenerateShiftID()
        {
            var selectStatement = "SELECT MAX(ShiftID) FROM dbo.ShiftSchedules";
            SqlCommand selectCommand = new SqlCommand(selectStatement, _connection);
            _connection.Open();
            var maxID = selectCommand.ExecuteScalar();
            _connection.Close();
            int newID = (maxID != DBNull.Value) ? Convert.ToInt32(maxID) + 1 : 1;
            return newID;
        }
        public void UpdateShiftSchedule(ShiftSchedule shift)
        {
            var updateStatement = "UPDATE dbo.ShiftSchedules SET ShiftName = @ShiftName, ShiftStartTime = @ShiftStartTime, ShiftEndTime = @ShiftEndTime WHERE ShiftID = @ShiftID";
            SqlCommand updateCommand = new SqlCommand(updateStatement, _connection);
            updateCommand.Parameters.AddWithValue("@ShiftName", shift.ShiftName);
            updateCommand.Parameters.AddWithValue("@ShiftStartTime", shift.ShiftStartTime);
            updateCommand.Parameters.AddWithValue("@ShiftEndTime", shift.ShiftEndTime);
            updateCommand.Parameters.AddWithValue("@ShiftID", shift.ShiftID);
            _connection.Open();
            updateCommand.ExecuteNonQuery();
            _connection.Close();
        }
        public void DeleteShiftSchedule(int shiftID)
        {
            var deleteStatement = "DELETE FROM dbo.ShiftSchedules WHERE ShiftID = @ShiftID";
            SqlCommand deleteCommand = new SqlCommand(deleteStatement, _connection);
            deleteCommand.Parameters.AddWithValue("@ShiftID", shiftID);
            _connection.Open();
            deleteCommand.ExecuteNonQuery();
            _connection.Close();
        }
    }
}
