using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeepingModels
{
    public class ShiftSchedule
    {
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
        public TimeOnly ShiftStartTime { get; set; }
        public TimeOnly ShiftEndTime { get; set; }
        
    }
}
