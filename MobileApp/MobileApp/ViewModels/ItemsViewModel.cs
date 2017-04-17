
namespace MobileApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
		public string Address { get; set; }

		public ItemsViewModel()
		{
			Title = "Browse";
			Address = "192.168.0.1";
		}
	}
}
