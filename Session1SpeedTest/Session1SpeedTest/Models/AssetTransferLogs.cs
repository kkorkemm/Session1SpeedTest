using System;
using System.Collections.Generic;
using System.Text;

namespace Session1SpeedTest.Models
{
    public class AssetTransferLogs
    {
        public int ID { get; set; }
        public int AssetID { get; set; }
        public DateTime TransferDate { get; set; }
        public string FromAssetSN { get; set; }
        public string ToAssetSN { get; set; }
        public int FromDepartmentLocationID { get; set; }
        public int ToDepartmentLocationID { get; set; }
        public string FromDepartment { get; set; }
        public string ToDepartment { get; set; }
    }
}
