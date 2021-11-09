using System;
using System.Collections.Generic;
using System.Text;

namespace Session1SpeedTest.Models
{
    public class Assets
    {
        public int ID { get; set; }
        public string AssetSN { get; set; }
        public string AssetName { get; set; }
        public int DepartmentLocationID { get; set; }
        public int EmployeeID { get; set; }
        public int AssetGroupID { get; set; }
        public string Description { get; set; }
        public DateTime? WarrantyDate { get; set; }
        public string DepartmentName { get; set; }
        public long LocationID { get; set; }

        public bool IsVisible { get; set; }
    }
}
