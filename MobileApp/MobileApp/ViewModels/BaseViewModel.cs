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
	}
}
