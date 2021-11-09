using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Session1SpeedTest.Pages
{
    using Models;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        private Assets CurrentAsset = new Assets();
        public HistoryPage(Assets asset)
        {
            InitializeComponent();

            CurrentAsset = asset;
        }

        /// <summary>
        /// При загрузке страницы
        /// </summary>
        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            List<AssetTransferLogs> logs = AppData.GetTransferLogs().Where(p => p.AssetID == CurrentAsset.ID).ToList();

            if (logs.Count() < 1)
            {
                await DisplayAlert("Warning!", "Нет записей для этого актива", "Ok");
                await Navigation.PopAsync();
            }
            else
            {
                ListHistory.ItemsSource = logs;
            }
        }

        /// <summary>
        /// Назад
        /// </summary>
        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}