using HorizontalList;
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
			ViewModel.View = this;
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			ClearFolder();
			await Navigation.PopAsync();
		}

		private async void ClearFolder()
		{
			IEnumerable<string> collection = await DependencyService.Get<IFileWorker>().GetFilesAsync();
			foreach (string source in collection)
			{
				await DependencyService.Get<IFileWorker>().DeleteAsync(source);
			}
		}

		private void CarouselImages_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			CarouselItem item = e.SelectedItem as CarouselItem;
			if (item == null)
			{
				return;
			}
			ViewModel.CurrentItem = item;
		}	
	}
}
