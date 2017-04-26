namespace MobileApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
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
			Address = "192.168.0.1";
		}
	}
}
