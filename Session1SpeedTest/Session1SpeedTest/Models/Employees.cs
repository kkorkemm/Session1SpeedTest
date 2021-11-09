using System;
using System.Collections.Generic;
using System.Text;

namespace Session1SpeedTest.Models
{
    public class Employees
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public string FullName => FirstName + " " + LastName;
    }
}
