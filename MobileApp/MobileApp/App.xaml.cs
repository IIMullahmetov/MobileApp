using MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MobileApp
{
	public partial class App : Application
	{

		public static int Height { get; set; }
		public static int Width { get; set; }
		public App ()
		{
			InitializeComponent();
			ContentPage page = new ConnectionPage();
			MainPage = new NavigationPage(page)
			{
				Title = string.Empty,
#pragma warning disable CS0618 // Тип или член устарел
				Icon = Device.OnPlatform("tab_feed.png", null, null)
#pragma warning restore CS0618 // Тип или член устарел
			};
			

			//	new TabbedPage
			//	{
			//		Children =
			//	{

			//	}
			//};

		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
