using BackTest.Data;
using BackTest.Framework;
using System.Windows;
using Index = BackTest.Reference.Index;

namespace BackTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Use this as composition root since this is a siple
        // application which won't need a DI container
        public MainWindow()
        {
            // I don't have the rights to redistribute the data, so load
            // from a neutral location
            var dataSource = new FileSpecificDataSource(@"C:\Temp\BackTestData");

            var marketData = new MarketData(dataSource);
            var marketAtTime = new MarketAtTime(marketData);

            WindowState = WindowState.Maximized;
            DataContext = new MainWindowVM(marketAtTime, marketData, new List<IPriceSeries>()
            {
                Index.WholeMarket(marketAtTime),
                Index.Top(marketAtTime, 100),
            });

            InitializeComponent();
        }
    }
}