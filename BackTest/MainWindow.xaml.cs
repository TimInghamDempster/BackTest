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

            WindowState = WindowState.Maximized;
            DataContext = new MainWindowVM();
            InitializeComponent();
        }
    }
}