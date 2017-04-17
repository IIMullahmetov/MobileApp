
using Xamarin.Forms;

namespace MobileApp.Models
{
    public class CarouselItem : Image
    {
		private string source;
		public string SourceOfImage
		{
			get => source;
			set => source = value;
		}
	}
}
