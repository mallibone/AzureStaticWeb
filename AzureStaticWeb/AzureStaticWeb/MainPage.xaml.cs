using AzureStaticWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AzureStaticWeb
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }


        QuotesViewModel ViewModel { get; } = new QuotesViewModel();

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Init();
        }
    }
}
