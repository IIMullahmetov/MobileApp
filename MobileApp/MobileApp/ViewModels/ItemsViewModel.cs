using MobileApp.Services;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
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
		private string address;
		public string Address
		{
			get => address;
			set
			{
				address = value;
				OnPropertyChanged("Address");
			}
		}
		
		public ItemsViewModel()
		{
			Title = "Browse";
			Address = "192.168.0.105";
		}

		public async void ClearFolder()
		{
			IEnumerable<string> collection = await DependencyService.Get<IFileWorker>().GetFilesAsync();
			foreach (string source in collection)
			{
				await DependencyService.Get<IFileWorker>().DeleteAsync(source);
			}
		}
	}
}
