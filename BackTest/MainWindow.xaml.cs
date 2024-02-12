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
            // Use this as composition root since this is a siple
            // application which won't need a DI container
            var marketData = new MarketData(new List<CompanyData>());

            WindowState = WindowState.Maximized;
            DataContext = new MainWindowVM(marketData);
            InitializeComponent();
        }
    }
}