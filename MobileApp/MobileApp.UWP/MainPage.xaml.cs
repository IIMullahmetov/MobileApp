using Windows.Foundation;
using Windows.UI.Xaml;

namespace MobileApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
			Rect windowSize = Window.Current.Bounds;
			double windowHeight = windowSize.Height;
			double windowWidth = windowSize.Width;
			LoadApplication(new MobileApp.App());
			MobileApp.App.Height = (int)windowHeight;
			MobileApp.App.Width = (int)windowWidth;
        }
    }
}
