using System.IO;
using Xamarin.Forms;

namespace MobileApp.Services
{
    public class ClientImageConverter
    {
		public static ImageSource ByteArrayToImage(byte[] byteImage) => ImageSource.FromStream(() => new MemoryStream(byteImage)); //превращаем массив байт в изображение
	}
}
