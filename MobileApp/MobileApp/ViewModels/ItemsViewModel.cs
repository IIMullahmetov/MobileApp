using System.Net;

namespace MobileApp.ViewModels
{
	public class ItemsViewModel : BaseViewModel
    {
		private bool isCorrect = false;

		public bool IsCorrect
		{
			get => isCorrect;
			set
			{
				isCorrect = IPAddress.TryParse(Address, out IPAddress address);
				OnPropertyChanged("IsCorrect");
			}
		}

		private string title;
		public string Title
		{
			get => title;
			set
			{
				title = value;
				OnPropertyChanged("Title");
			}
		}
		private string address = string.Empty;
		public string Address
		{
			get => address;
			set
			{
				address = value;
				IsCorrect = false;
				OnPropertyChanged("Address");
			}
		}
		
		public ItemsViewModel()
		{
			Title = "Browse";
			Address = "192.168.137.131";
		}
	}
}
