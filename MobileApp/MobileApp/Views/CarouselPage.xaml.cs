using MobileApp.Models;
using MobileApp.Services;
using MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CarouselPage : ContentPage
	{
		private CarouselPageViewModel ViewModel { get; set; }

		public CarouselPage(CarouselPageViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = ViewModel = viewModel;
			//Car.SelectedIndex = ViewModel.Position;
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync();
		}
	}
}
