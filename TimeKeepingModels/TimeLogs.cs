using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeepingModels
{
    public class TimeLogs
    {
        public int EmployeeID { get; set; }
        public string ShiftName { get; set; }
        public DateOnly Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public TimeSpan WorkingHours { get; set; }
        public TimeSpan LateHours { get; set; }
        public TimeSpan OvertimeHours { get; set; }
        public TimeSpan UndertimeHours { get; set; }
    }
}
