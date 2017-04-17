using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
