using MobileApp.Models;
using MobileApp.Services;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class CarouselPageViewModel : BaseViewModel
    {
		public Views.CarouselPage View { get; set; }
		public int Height => App.Height / 2;
		public ObservableCollection<CarouselItem> Images { get; set; }
		private CarouselItem currentItem;
		public CarouselItem CurrentItem
		{
			get => currentItem;
			set
			{
				PreviousItem = CurrentItem;
				currentItem = value;
				CurrentSlide = Images.IndexOf(currentItem) + 1;
				SendCommand();
			}
		}
		public CarouselItem PreviousItem { get; set; }
		private string Address { get; set; }
		private ClientConnection cc;
		private int currentSlide = 1;
		public int CurrentSlide
		{
			get => currentSlide;
			set
			{
				currentSlide = value;
				try
				{
					currentItem = Images[currentSlide - 1];
					Position = value - 1;
				}
				catch { }
				OnPropertyChanged("CurrentSlide");
			}
		}
		
		public CarouselPageViewModel(string address)
		{
			Images = new ObservableCollection<CarouselItem>();
			cc = new ClientConnection(1024, 4); 
			Address = address;
			AsyncConnection();
			PlayCommand = new Command(() => AsyncRequest(-3));
			StopCommand = new Command(() => AsyncRequest(-4));
			ExitCommand = new Command(() => AsyncRequest(-5));
		}
		public ICommand LoadItemsCommand { get; set; }
		public ICommand ExitCommand { get; set; }
		public ICommand PlayCommand { get; set; }
		public ICommand StopCommand { get; set; }
		private async void AsyncConnection()
		{
			List<ImageSource> list = await cc.Connection(Address);
			foreach (ImageSource source in list)
			{
				Images.Add(new CarouselItem() { Source = source });
			}
			IEnumerable<string> collection = await DependencyService.Get<IFileWorker>().GetFilesAsync();
			string path = DependencyService.Get<IFileWorker>().GetLocalFolderPath() + "\\";
			foreach (string source in collection)
			{
				//DependencyService.Get<IFileWorker>().DeleteAsync(source);
				Images.Add(new CarouselItem() { Source = path + source, SourceOfImage = path + source });
			}

			Title = cc.Title;
			View.SetElements();
			View.ChangeView();
		}

		private void SendCommand()
		{
			int number = Images.IndexOf(PreviousItem) - Images.IndexOf(CurrentItem);
			switch (number)
			{
				case -1:
					AsyncRequest(-1);
					break;
				case 1:
					AsyncRequest(-2);
					break;
				default:
					AsyncRequest(CurrentSlide);
					break;
			}
		}

		private async void AsyncRequest(int message)
		{
			try
			{
				int code = await cc.Request(message);
				//Console.WriteLine(code);
			}
			catch
			{
				//Console.WriteLine(ex.Message);
			}
		}

		private int _position = 0;
		public int Position
		{
			get => _position;
			set { _position = value; OnPropertyChanged(); }
		}
	}
}
