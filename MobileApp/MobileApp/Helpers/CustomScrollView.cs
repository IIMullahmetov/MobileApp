using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileApp.Helpers
{
	public class CustomScrollView : ScrollView
	{
		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(CustomScrollView), default(IEnumerable));

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(CustomScrollView), default(DataTemplate));

		public DataTemplate ItemTemplate
		{
			get => (DataTemplate)GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		public event EventHandler<ItemTappedEventArgs> ItemSelected;

		public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create("SelectedCommand", typeof(ICommand), typeof(CustomScrollView), null);

		public ICommand SelectedCommand
		{
			get => (ICommand)GetValue(SelectedCommandProperty);
			set => SetValue(SelectedCommandProperty, value);
		}

		public static readonly BindableProperty SelectedCommandParameterProperty = BindableProperty.Create("SelectedCommandParameter", typeof(object), typeof(CustomScrollView), null);

		public object SelectedCommandParameter
		{
			get => GetValue(SelectedCommandParameterProperty);
			set => SetValue(SelectedCommandParameterProperty, value);
		}

		public void Render()
		{
			if (ItemTemplate == null || ItemsSource == null)
			{
				return;
			}

			StackLayout layout = new StackLayout
			{
				Orientation = Orientation == ScrollOrientation.Vertical ? StackOrientation.Vertical : StackOrientation.Horizontal
			};

			foreach (object item in ItemsSource)
			{
				ICommand command = SelectedCommand ?? new Command((obj) =>
				{
					ItemTappedEventArgs args = new ItemTappedEventArgs(ItemsSource, item);
					ItemSelected?.Invoke(this, args);
				});
				object commandParameter = SelectedCommandParameter ?? item;

				ViewCell viewCell = ItemTemplate.CreateContent() as ViewCell;
				viewCell.View.BindingContext = item;
				viewCell.View.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = command,
					CommandParameter = commandParameter,
					NumberOfTapsRequired = 1
				});

				layout.Children.Add(viewCell.View);
			}

			Content = layout;
		}
	}
}