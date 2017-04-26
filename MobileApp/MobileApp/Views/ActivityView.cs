using Xamarin.Forms;

namespace MobileApp.Views
{
	public class ActivityView : ContentView
	{
		public ActivityView()
		{
			StackLayout layout = new StackLayout();
			layout.Children.Add(new Label() { Text = "Loading...", HorizontalOptions = LayoutOptions.Center,  });
			layout.Children.Add(new ActivityIndicator() { Color = Color.Black } );
			Content = layout;
		}			
	}
}
