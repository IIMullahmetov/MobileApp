using MobileApp.Models;
using MobileApp.Services;
using PCLStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class CarouselPageViewModel : BaseViewModel
    {
		public int Height => App.Height / 4;
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
		private ClientConnection cc = new ClientConnection(1024, 4);
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
				catch
				{
					Position = Images.Count;
					CurrentSlide = Position;
				}		
				OnPropertyChanged("CurrentSlide");
			}
		}
		public Command LoadItemsCommand { get; set; }
		public CarouselPageViewModel(string address)
		{
			Images = new ObservableCollection<CarouselItem>();
			Address = address;
			LoadItemsCommand = new Command(async () => await AsyncConnection());
			for (int i = 1; i < 10; i++)
			{
				Images.Add(new CarouselItem()
				{
#pragma warning disable CS0618 // Тип или член устарел
					SourceOfImage =
					Device.OnPlatform(
						iOS: "Slides/Image_0" + i + ".png",
						Android: "Image_0" + i + ".png",
						WinPhone: "Resources/Slides/Image_0" + i + ".png"
				)
#pragma warning restore CS0618 // Тип или член устарел
				});
			}

			PlayCommand = new Command(() => AsyncRequest(-3));
			StopCommand = new Command(() => AsyncRequest(-4));
			ExitCommand = new Command(() => AsyncRequest(-5));
			CurrentSlide = 1;
		}
		public ICommand ExitCommand { get; set; }
		public ICommand PlayCommand { get; set; }
		public ICommand StopCommand { get; set; }
		private async Task AsyncConnection()
		{
			try
			{
				List<ImageSource> v = await cc.Connection(Address);
			}
			catch { }

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
					AsyncRequest(1);
					break;
				default:
					AsyncRequest(number);
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

		private int _position;
		public int Position
		{
			get => _position;
			set { _position = value; OnPropertyChanged(); }
		}
	}
}
