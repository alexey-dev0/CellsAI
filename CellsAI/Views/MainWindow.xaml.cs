using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			GameMain.Reset();
		}

		public void AddSlider(string name, double min, double max, double defVal)
		{
			var slider = new Slider()
			{
				Minimum = min,
				Maximum = max,
				Value = defVal
			};
			var textbox = new TextBox() { Width = 40, Text = $"{defVal:F2}" };
			_sliderBox.Add(slider, textbox);
			slider.ValueChanged += Slider_ValueChanged;
			DockPanel.SetDock(textbox, Dock.Right);
			Sliders.Add(name, slider);

			StackPnl.Children.Add(new Label() { Content = name });
			var dockPnl = new DockPanel() { VerticalAlignment = VerticalAlignment.Center };
			dockPnl.Children.Add(textbox);
			dockPnl.Children.Add(slider);
			StackPnl.Children.Add(dockPnl);
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_sliderBox[sender as Slider].Text = $"{(sender as Slider).Value:F2}";
		}

		private static Dictionary<Slider, TextBox> _sliderBox = new Dictionary<Slider, TextBox>();
	}
}