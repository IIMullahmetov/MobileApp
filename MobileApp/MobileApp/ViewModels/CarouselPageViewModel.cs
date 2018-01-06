using MobileApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
	public class CarouselPageViewModel : BaseViewModel
	{
		public string Title => cc.Title;
		public int Count => cc.SlidesCount;
		
		public ObservableCollection<CarouselItem> Minis { get; set; }

		public ObservableCollection<CarouselItem> Images => cc.Items;
		private CarouselItem currentItem;
		public CarouselItem CurrentItem
		{
			get => currentItem;
			set
			{
				try
				{
					currentItem = value;
					Position = Images.IndexOf(currentItem);
					OnPropertyChanged("CurrentItem");
				}
				catch { }
			}
		}

		private string Address { get; set; }
		private ClientConnection cc = new ClientConnection(1024, 4);
		public int CurrentSlide
		{
			get => Position + 1;
			set
			{
				if (value > 0 && value <= Images.Count)
				{
					Position = value - 1;
				}
				OnPropertyChanged("CurrentSlide");
			}
		}

		public CarouselPageViewModel(string address)
		{
			Minis = new ObservableCollection<CarouselItem>();
			Address = address;
			AsyncConnection();
			PlayCommand = new Command(() => AsyncRequest(-3));
			StopCommand = new Command(() => { Position = 1; AsyncRequest(-4); });
		}
		
		public ICommand PlayCommand { get; private set; }
		public ICommand StopCommand { get; private set; }

		private async void AsyncConnection()
		{
			await cc.Connection(Address);
			for (int i = 1; i < Images.Count; i++)
			{
				Minis.Add(Images[i]);
			}
		}

		private void SendCommand(int number)
		{
			switch (number)
			{
				case -1:
					AsyncRequest(-1);
					break;
				default:
					AsyncRequest(Position);
					break;
			}
		}
		
		private async void AsyncRequest(int message)
		{
			if (await cc.Request(message) == -1)
			{
				currentItem = Images[Position];
			}
		}

		private int _position;
		public int Position
		{
			get => _position;
			set
			{
				if (value > 0 && value <= Images.Count)
				{
					_position = value;
					SendCommand(value);
				}
				OnPropertyChanged("Position");
			}
		}

	}
}
