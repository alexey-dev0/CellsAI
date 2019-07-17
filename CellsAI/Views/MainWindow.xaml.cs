using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CellsAI.Views
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static Dictionary<string, Slider> Sliders = new Dictionary<string, Slider>();

		public MainWindow()
		{
			InitializeComponent();
			GameMain.Win = this;
		}

		public void AddSlider(string name, double min, double max, double defVal)
		{
			var slider = new Slider()
			{
				Minimum = min,
				Maximum = max,
				Value = defVal,
				IsSnapToTickEnabled = true
			};
			var textbox = new TextBox()
			{
				Background = new SolidColorBrush(Color.FromRgb(0x3E, 0x3E, 0x3E)),
				Foreground = new SolidColorBrush(Color.FromRgb(0xE8, 0xE8, 0xE8)),
				Width = 40,
				Text = $"{defVal:F0}"
			};
			_sliderBox.Add(slider, textbox);
			slider.ValueChanged += Slider_ValueChanged;
			DockPanel.SetDock(textbox, Dock.Right);
			Sliders.Add(name, slider);

			StackPnl.Children.Insert(1, new Label()
			{
				Foreground = new SolidColorBrush(Color.FromRgb(0xE8, 0xE8, 0xE8)),
				Content = name
			});
			var dockPnl = new DockPanel() { VerticalAlignment = VerticalAlignment.Center };
			dockPnl.Children.Add(textbox);
			dockPnl.Children.Add(slider);
			StackPnl.Children.Insert(2, dockPnl);
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double value = (sender as Slider).Value;
			_sliderBox[sender as Slider].Text = value <= 0 ? $"1/{-value + 2:F0}" : $"{value:F0}";
		}

		private static readonly Dictionary<Slider, TextBox> _sliderBox = new Dictionary<Slider, TextBox>();
	}
}