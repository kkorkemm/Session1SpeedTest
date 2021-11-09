using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Session1SpeedTest
{
    using Models;

    /// <summary>
    /// Работа с API
    /// </summary>
    public class AppData
    {
        private static string addressPhone = "http://192.168.0.108:55017/api/";
        private static string addressEmulator = "http://10.0.2.2:55017/api/";

        /// <summary>
        /// Проверка устройства
        /// </summary>
        /// <returns> нужный адрес api </returns>
        public static string CheckDevice()
        {
            string address;
            try
            {
                var responce = new WebClient().DownloadString($"{addressPhone}Assets");
                address = addressPhone;
            }
            catch
            {
                address = addressEmulator;
            }
            return address;
        }

        public static List<Assets> GetAssets()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}Assets");
            return JsonConvert.DeserializeObject<List<Assets>>(responce);
        }

        public static List<AssetGroups> GetAssetGroups()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}AssetGroups");
            return JsonConvert.DeserializeObject<List<AssetGroups>>(responce);
        }

        public static List<AssetPhotos> GetAssetPhotos()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}AssetPhotos");
            return JsonConvert.DeserializeObject<List<AssetPhotos>>(responce);
        }

        public static List<AssetTransferLogs> GetTransferLogs()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}AssetTransferLogs");
            return JsonConvert.DeserializeObject<List<AssetTransferLogs>>(responce);
        }

        public static List<DepartmentLocations> GetDepartmentLocations()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}DepartmentLocations");
            return JsonConvert.DeserializeObject<List<DepartmentLocations>>(responce);
        }

        public static List<Departments> GetDepartments()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}Departments");
            return JsonConvert.DeserializeObject<List<Departments>>(responce);
        }

        public static List<Employees> GetEmployees()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}Employees");
            return JsonConvert.DeserializeObject<List<Employees>>(responce);
        }

        public static List<Locations> GetLocations()
        {
            string address = CheckDevice();
            var responce = new WebClient().DownloadString($"{address}Locations");
            return JsonConvert.DeserializeObject<List<Locations>>(responce);
        }
    }
}
