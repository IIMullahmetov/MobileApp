using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MobileApp.Droid
{
	[Activity (Label = "MobileApp", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;
			base.OnCreate (bundle);
			Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new MobileApp.App ());
			Android.Util.DisplayMetrics v = Resources.DisplayMetrics;
			App.Height = v.HeightPixels;
			App.Width = v.WidthPixels;
		}
	}
}

