using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Session1SpeedTest.Pages
{
    using Models;
    using System.Net;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditAssetPage : ContentPage
    {
        private Assets currentAsset = new Assets();
        private WebClient client = new WebClient();
        private string address = AppData.CheckDevice();

        public AddEditAssetPage(Assets asset)
        {
            InitializeComponent();

            #region Pickers
            List<Departments> departments = AppData.GetDepartments();
            List<AssetGroups> groups = AppData.GetAssetGroups();
            List<Employees> employees = AppData.GetEmployees();
            List<Locations> locations = AppData.GetLocations();

            ComboDepartments.ItemsSource = departments;
            ComboGroups.ItemsSource = groups;
            ComboEmployees.ItemsSource = employees;
            ComboLocations.ItemsSource = locations;
            #endregion

            if (asset != null)
            {
                currentAsset = asset;

                ComboDepartments.SelectedItem = departments.FirstOrDefault(p => p.Name == currentAsset.DepartmentName);
                ComboEmployees.SelectedItem = employees.FirstOrDefault(p => p.ID == currentAsset.EmployeeID);
                ComboLocations.SelectedItem = locations.FirstOrDefault(p => p.ID == currentAsset.LocationID);
                ComboGroups.SelectedItem = groups.FirstOrDefault(p => p.ID == currentAsset.AssetGroupID);

                ComboDepartments.IsEnabled = false;
                ComboEmployees.IsEnabled = false;
                ComboGroups.IsEnabled = false;
            }

            BindingContext = currentAsset;
        }

        private void BtnCapture_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnBrowse_Clicked(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Сохранение
        /// </summary>
        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(currentAsset.AssetName))
                errors.AppendLine("Укажите имя актива");
            if (ComboDepartments.SelectedItem == null)
                errors.AppendLine("Укажите отдел");
            if (ComboGroups.SelectedItem == null)
                errors.AppendLine("Укажите группу");
            if (ComboLocations.SelectedItem == null)
                errors.AppendLine("Укажите местоположение");
            if (ComboEmployees.SelectedItem == null)
                errors.AppendLine("Укажите сотрудника");
            if (string.IsNullOrWhiteSpace(currentAsset.AssetSN))
                errors.AppendLine("Поле Asset SN не может быть пустым");
            if (string.IsNullOrWhiteSpace(currentAsset.Description))
                errors.AppendLine("Добавьте описание");

            if (errors.Length > 0)
            {
                await DisplayAlert("Error!", errors.ToString(), "Ok");
                return;
            }

            // создание нового DepartmentLocations
            DepartmentLocations departmentLocations = new DepartmentLocations()
            {
                DepartmentID = (ComboDepartments.SelectedItem as Departments).ID,
                LocationID = (ComboLocations.SelectedItem as Locations).ID,
                StartDate = DateTime.Now
            };

            try
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = client.UploadString($"{address}DepartmentLocations", JsonConvert.SerializeObject(departmentLocations));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error!", ex.Message, "Ok");
                return;
            }

            /// получение ID только что созданного DepartmentLocations
            DepartmentLocations lastDL = AppData.GetDepartmentLocations().OrderBy(p => p.ID).LastOrDefault();

            Assets newAsset = new Assets()
            {
                AssetName = currentAsset.AssetName,
                AssetSN = currentAsset.AssetSN,
                DepartmentLocationID = lastDL.ID,
                EmployeeID = (ComboEmployees.SelectedItem as Employees).ID,
                AssetGroupID = (ComboGroups.SelectedItem as AssetGroups).ID,
                Description = currentAsset.Description,
                WarrantyDate = currentAsset.WarrantyDate
            };

            if (currentAsset.ID == 0)
            {
                try
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    var result = client.UploadString($"{address}Assets", JsonConvert.SerializeObject(newAsset));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error!", ex.Message, "Ok");
                    return;
                }
            }
            else
            {
                try
                {
                    newAsset.ID = currentAsset.ID;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    var result = client.UploadString($"{address}Assets/{currentAsset.ID}", "PUT", JsonConvert.SerializeObject(newAsset));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error!", ex.Message, "Ok");
                    return;
                }
            }

            await DisplayAlert("Success!", "Asset saved!", "Ok");
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Назад
        /// </summary>
        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void UpdateNewSN()
        {
            string number = "";

            Departments department = ComboDepartments.SelectedItem as Departments;
            AssetGroups locations = ComboGroups.SelectedItem as AssetGroups;

            if (department != null)
            {
                if (department.ID < 10)
                    number += $"0{department.ID}/";
                else
                    number += $"{department.ID}/";
            }
            else
            {
                number += "dd/";
            }

            if (locations != null)
            {
                if (locations.ID < 10)
                    number += $"0{locations.ID}/";
                else
                    number += $"{locations.ID}/";
            }
            else
            {
                number += "gg/";
            }

            if (department != null && locations != null)
            {
                int count = AppData.GetAssets().Where(p => p.DepartmentName == department.Name && p.AssetGroupID == locations.ID).ToList().Count() + 1;

                if (count < 10)
                    number += $"000{count}";
                else if (count < 100)
                    number += $"00{count}";
                else if (count < 1000)
                    number += $"0{count}";
                else
                    number += $"{count}";
            }
            else
            {
                number += "nnnn";
            }

            currentAsset.AssetSN = number;
            TextNewSN.Text = number;
        }

        private void ComboDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentAsset.ID == 0)
                UpdateNewSN();
        }

        private void ComboGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentAsset.ID == 0)
                UpdateNewSN();
        }
    }
}