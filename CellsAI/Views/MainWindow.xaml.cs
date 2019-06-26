using System.Windows;

namespace CellsAI.Views
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			GameMain.Reset();
		}
	}
}