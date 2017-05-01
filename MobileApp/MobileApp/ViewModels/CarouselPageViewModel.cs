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
		public string Title
		{
			get => cc.Title;
		}
		public string Path { get; private set; } = DependencyService.Get<IFileWorker>().GetLocalFolderPath() + Device.OnPlatform(iOS: "/", Android: "/", WinPhone: "\\");
		public Views.CarouselPage View { get; set; }
		public int Height => App.Height;
		public int Width => App.Width / 3;
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
		private int currentSlide;
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
			Address = address;
			AsyncConnection();
			CurrentSlide = 1;
			PlayCommand = new Command(() => AsyncRequest(-3));
			StopCommand = new Command(() => AsyncRequest(-4));
			ExitCommand = new Command(async() => 
			{
				AsyncRequest(-5);
				IEnumerable<string> collection = await DependencyService.Get<IFileWorker>().GetFilesAsync();
				foreach (string source in collection)
				{
					DependencyService.Get<IFileWorker>().DeleteAsync(source);
				}
				Images.Clear();
				cc.Shutdown();
			});
			StartCommand = new Command(()=>
			{
				cc = null;
				cc = new ClientConnection(1024, 4);
				AsyncConnection();
			});
		}
		public ICommand LoadItemsCommand { get; set; }
		public ICommand ExitCommand { get; set; }
		public ICommand PlayCommand { get; set; }
		public ICommand StopCommand { get; set; }
		public ICommand StartCommand { get; set; }
		public void SetElement(string source)
		{
			lock (Images)
			{
				CarouselItem item = new CarouselItem() { Source = Path + source };
				Images.Add(item);
			}
		}

		private async void AsyncConnection()
		{
			cc = new ClientConnection(1024, 4)
			{
				ViewModel = this
			};
			await cc.Connection(Address);
			CurrentSlide = 1;
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
			}
			catch { }
		}

		private int _position = 0;
		public int Position
		{
			get => _position;
			set { _position = value; OnPropertyChanged(); }
		}
	}
}
