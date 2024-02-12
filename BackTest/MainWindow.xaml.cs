using System.Windows;

namespace BackTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var marketData = new MarketData();

            WindowState = WindowState.Maximized;
            DataContext = new MainWindowVM(marketData);
            InitializeComponent();
        }
    }
}