using System;
using System.Collections.Generic;
using System.Text;

namespace Session1SpeedTest.Models
{
    public class DepartmentLocations
    {
        public int ID { get; set; }
        public int DepartmentID { get; set; }
        public int LocationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
