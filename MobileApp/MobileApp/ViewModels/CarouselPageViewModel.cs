using MobileApp.Models;
using MobileApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class CarouselPageViewModel : BaseViewModel
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
		private string path = DependencyService.Get<IFileWorker>().GetLocalFolderPath() + Device.OnPlatform(iOS: "/", Android: "/", WinPhone: "\\");
		public ObservableCollection<Item> Items { get; set; }
		public Views.CarouselPage View { get; set; }
		public int Height => App.Height;
		public int Width => App.Width;
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
			Items = new ObservableCollection<Item>();
			Images = new ObservableCollection<CarouselItem>();
			cc = new ClientConnection(1024, 4)
			{
				ViewModel = this
			};
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

		public void SetElement(string source)
		{
			lock (Images)
			{
				Images.Add(new CarouselItem() { Source = path + source });
			}
		}
		private async void AsyncConnection()
		{
			await cc.Connection(Address);
			//IEnumerable<string> list = await DependencyService.Get<IFileWorker>().GetFilesAsync();
			//foreach (string source in list)
			//{
			//	CarouselItem item = new CarouselItem() { Source = path + source};
			//	Images.Add(item);
			//}
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
