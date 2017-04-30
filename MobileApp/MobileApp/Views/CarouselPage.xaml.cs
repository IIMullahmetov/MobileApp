using MobileApp.Models; 
using MobileApp.ViewModels;
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
