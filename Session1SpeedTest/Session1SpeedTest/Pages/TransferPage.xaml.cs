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
    public partial class TransferPage : ContentPage
    {
        private Assets CurrentAsset = new Assets();
        private WebClient client = new WebClient();
        private string address = AppData.CheckDevice();

        public TransferPage(Assets asset)
        {
            InitializeComponent();

            CurrentAsset = asset;

            /// Pickers
            List<Departments> departments = AppData.GetDepartments();
            ComboDestDepartments.ItemsSource = departments.ToList();
            ComboDepartments.ItemsSource = departments.ToList();

            ComboDepartments.SelectedItem = departments.Where(p => p.Name == asset.DepartmentName).FirstOrDefault();

            List<Locations> locations = AppData.GetLocations();
            ComboDestLocations.ItemsSource = locations;

            BindingContext = CurrentAsset;
        }

        /// <summary>
        /// Формирование нового Asset SN
        /// </summary>
        private void ComboDestDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            string number = "";

            Departments department = ComboDestDepartments.SelectedItem as Departments;

            AssetTransferLogs logs = AppData.GetTransferLogs().Where(p => p.AssetID == CurrentAsset.ID && p.ToDepartment == department.Name).FirstOrDefault();

            if (logs != null)
            {
                TextNewSN.Text = logs.ToAssetSN;
                return;
            }

            if (department.ID < 10)
                number += $"0{department.ID}/";
            else
                number += $"{department.ID}/";

            if (CurrentAsset.AssetGroupID < 10)
                number += $"0{CurrentAsset.AssetGroupID}/";
            else
                number += $"{CurrentAsset.AssetGroupID}/";

            int count = AppData.GetAssets().Where(p => p.DepartmentName == CurrentAsset.DepartmentName && p.AssetGroupID == CurrentAsset.AssetGroupID).ToList().Count() + 1;

            if (count < 10)
                number += $"000{count}";
            else if (count < 100)
                number += $"00{count}";
            else if (count < 1000)
                number += $"0{count}";
            else
                number += $"{count}";

            TextNewSN.Text = number;
        }

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            Departments selectedDepartment = ComboDestDepartments.SelectedItem as Departments;

            /// проверка на заполнение полей
            if (selectedDepartment == null)
                errors.AppendLine("Укажите отдел назначения");
            if (ComboDestLocations.SelectedItem == null)
                errors.AppendLine("Укажите место назначения");
            if (string.IsNullOrWhiteSpace(TextNewSN.Text))
                errors.AppendLine("Поле Asset SN не может быть пустым");
            if (selectedDepartment != null && selectedDepartment.Name == CurrentAsset.DepartmentName)
                errors.AppendLine("Отдел назначения не может быть текущим отделом");

            if (errors.Length > 0)
            {
                await DisplayAlert("Error!", errors.ToString(), "Ok");
                return;
            }

            // создание нового DepartmentLocations
            DepartmentLocations departmentLocations = new DepartmentLocations()
            {
                DepartmentID = (ComboDestDepartments.SelectedItem as Departments).ID,
                LocationID = (ComboDestLocations.SelectedItem as Locations).ID,
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

            /// создание записи истории переводов
            AssetTransferLogs newLog = new AssetTransferLogs()
            {
                AssetID = CurrentAsset.ID,
                FromAssetSN = CurrentAsset.AssetSN,
                ToAssetSN = TextNewSN.Text,
                FromDepartmentLocationID = CurrentAsset.DepartmentLocationID,
                ToDepartmentLocationID = lastDL.ID,
                TransferDate = DateTime.Now
            };

            try
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = client.UploadString($"{address}AssetTransferLogs", JsonConvert.SerializeObject(newLog));

                await DisplayAlert("Success!", "Перевод выполнен!", "Ok");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error!", ex.Message, "Ok");
                return;
            }
        }

        /// <summary>
        /// Назад
        /// </summary>
        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}