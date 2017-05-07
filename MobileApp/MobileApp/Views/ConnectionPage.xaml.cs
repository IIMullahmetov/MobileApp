using MobileApp.Services;
using MobileApp.ViewModels;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConnectionPage : ContentPage
	{
		private ItemsViewModel viewModel;

		public ConnectionPage()
		{
			InitializeComponent();
			BindingContext = viewModel = new ItemsViewModel();
			viewModel.ClearFolder();
		}
		
		private async void Button_Clicked(object sender, EventArgs e) => 
			await Navigation.PushAsync(new CarouselPage(new CarouselPageViewModel(viewModel.Address)));
	}
}
