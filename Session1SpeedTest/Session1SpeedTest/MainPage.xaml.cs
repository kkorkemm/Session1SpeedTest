using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Session1SpeedTest
{
    using Models;

    public partial class MainPage : ContentPage
    {
        private List<Assets> assetList;

        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// При загрузке страницы
        /// </summary>
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            // Pickers
            List<Departments> departments = AppData.GetDepartments();
            List<AssetGroups> groups = AppData.GetAssetGroups();

            departments.Insert(0, new Departments { Name = "All" });
            groups.Insert(0, new AssetGroups { Name = "All" });

            ComboDepartments.ItemsSource = departments;
            ComboAssetGroups.ItemsSource = groups;

            DateStart.Date = DateTime.MinValue;

            UpdateList();
        }

        /// <summary>
        /// Обновление списка активов
        /// </summary>
        private void UpdateList()
        {
            assetList = AppData.GetAssets();
            int countAll = assetList.Count();


            if (ComboDepartments.SelectedIndex > 0)
            {
                Departments department = ComboDepartments.SelectedItem as Departments;
                assetList = assetList.Where(p => p.DepartmentName == department.Name).ToList();
            }
            if (ComboAssetGroups.SelectedIndex > 0)
            {
                AssetGroups group = ComboAssetGroups.SelectedItem as AssetGroups;
                assetList = assetList.Where(p => p.AssetGroupID == group.ID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(SearchBox.Text) && SearchBox.Text.Length > 2)
            {
                assetList = assetList.Where(p => p.AssetName.ToLower().Contains(SearchBox.Text.ToLower()) || p.AssetSN.ToString().StartsWith(SearchBox.Text)).ToList();
            }

            assetList = assetList.Where(p => (p.WarrantyDate >= DateStart.Date && p.WarrantyDate <= DateEnd.Date) || p.WarrantyDate == null).ToList();


            if (Width > Height)
            {
                foreach(var item in assetList)
                {
                    item.IsVisible = false;
                }
            }
            else
            {
                foreach (var item in assetList)
                {
                    item.IsVisible = true;
                }
            }

            ListAssets.ItemsSource = assetList;

            int countSelected = assetList.Count();

            TextCount.Text = $"{countSelected} assets from {countAll}";
        }

        #region Filters
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }

        private void DateStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            UpdateList();
        }

        private void ComboAssetGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void ComboDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        /// <summary>
        /// При смене ориентации экрана
        /// </summary>
        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {
            if (Width > Height)
            {
                GridPickers.IsVisible = false;
                GridDates.IsVisible = false;
                SearchBox.IsVisible = false;
                TextCount.IsVisible = false;
                TextTitle.Text = "Your assets: ";
            }
            else
            {
                GridPickers.IsVisible = true;
                GridDates.IsVisible = true;
                SearchBox.IsVisible = true;
                TextCount.IsVisible = true;
                TextTitle.Text = "Asset list: ";
            }

            UpdateList();
        }

        #endregion

        #region Navigation
        /// <summary>
        /// Редактирование актива
        /// </summary>
        private async void BtnEdit_Clicked(object sender, EventArgs e)
        {
            Assets selectedAsset = (sender as Button).BindingContext as Assets;
            await Navigation.PushAsync(new Pages.AddEditAssetPage(selectedAsset));
        }

        /// <summary>
        /// Переход на страницу перевода актива
        /// </summary>
        private async void BtnTransfer_Clicked(object sender, EventArgs e)
        {
            Assets selectedAsset = (sender as Button).BindingContext as Assets;
            await Navigation.PushAsync(new Pages.TransferPage(selectedAsset));
        }

        /// <summary>
        /// Переход на страницу историй переводов
        /// </summary>
        private async void BtnHistory_Clicked(object sender, EventArgs e)
        {
            Assets selectedAsset = (sender as Button).BindingContext as Assets;
            await Navigation.PushAsync(new Pages.HistoryPage(selectedAsset));
        }

        /// <summary>
        /// Добавление нового актива
        /// </summary>
        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.AddEditAssetPage(null));
        }
        #endregion
    }
}
