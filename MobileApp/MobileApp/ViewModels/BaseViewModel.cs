using MobileApp.Helpers;

namespace MobileApp.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
		bool isBusy = false;
		public bool IsBusy
		{
			get => isBusy; set => SetProperty(ref isBusy, value);
		}
		/// <summary>
		/// Private backing field to hold the title
		/// </summary>
		string title = string.Empty;
		/// <summary>
		/// Public property to set and get the title of the item
		/// </summary>
		public string Title
		{
			get => title; set => SetProperty(ref title, value);
		}
	}
}
