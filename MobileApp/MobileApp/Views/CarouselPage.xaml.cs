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
			foreach (CarouselItem image in ViewModel.Images)
			{
				TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
				tapGestureRecognizer.Tapped += (s, e) => {
					CarouselItem item = s as CarouselItem;
					ViewModel.Position = ViewModel.Images.IndexOf(item);
				};
				image.Source = image.SourceOfImage;
				image.GestureRecognizers.Add(tapGestureRecognizer);
				MainStack.Children.Add(image);
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

		private void SelectedItemOnListView(object sender, SelectedItemChangedEventArgs e)
		{
			CarouselItem item = e.SelectedItem as CarouselItem;
			ViewModel.Position = ViewModel.Images.IndexOf(item);
		}
	}
}
