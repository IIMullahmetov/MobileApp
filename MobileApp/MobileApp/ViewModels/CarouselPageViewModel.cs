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
		public int SlidesCount
		{
			get => cc.SlidesCount;
			set
			{
				cc.SlidesCount = value;
				OnPropertyChanged("SlidesCount");
			}
		}

		public ObservableCollection<CarouselItem> Miniatures { get; set; }

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
				SendCommand();
				Position = Images.IndexOf(currentItem);
			}
		}
		public CarouselItem PreviousItem { get; set; }	
		private string Address { get; set; }
		private ClientConnection cc;
		//public int CurrentSlide
		//{
		//	get => Position + 1;
		//	set
		//	{
		//		if (value > 0 && value <= Images.Count)
		//		{
		//			Position = value - 1;
		//		}			
		//		OnPropertyChanged("CurrentSlide");
		//	}
		//}
		
		public CarouselPageViewModel(string address)
		{
			Miniatures = new ObservableCollection<CarouselItem>();
			Images = new ObservableCollection<CarouselItem>() { new CarouselItem() { Source = "Resources\\Slides\\Image_01.png" } };
			Address = address;
			AsyncConnection();
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
			CarouselItem item = new CarouselItem() { Source = Path + source };
			//View.Set(item.Source);
			Miniatures.Add(item);
			Images.Add(item);
		}

		private async void AsyncConnection()
		{
			cc = new ClientConnection(1024, 4)
			{
				ViewModel = this
			};
			await cc.Connection(Address);
		}

		private void SendCommand()
		{
			if(PreviousItem == null)
			{
				return;
			}
			int number;
			try
			{
				number = Images.IndexOf(PreviousItem) - Images.IndexOf(CurrentItem);
			}
			catch { return; }
			switch (number)
			{
				case -1:
					AsyncRequest(-1);
					break;
				case 1:
					AsyncRequest(-2);
					break;
				default:
					AsyncRequest(Position);
					break;
			}
		}

		private async void AsyncRequest(int message)
		{
			if(await cc.Request(message) == -1)
			{
				currentItem = Images[Position- 1];
			}
		}

		private int _position;
		public int Position
		{
			get => _position;
			set
			{
				if(value > 0 && value <= Images.Count)
				{
					_position = value;
				}
				OnPropertyChanged("Position");
			}
		}
	}
}
