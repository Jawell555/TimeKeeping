namespace TimeKeepingModels
{
    public class Employee
    {
        public Guid EmployeeID { get; set; }
        public int ShiftSchedule { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
    }
}
