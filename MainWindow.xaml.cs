using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // We set the DataContext in the XAML, so no code-behind is needed for that.
            // If you prefer to set it here, you would uncomment the following line:
            // DataContext = new MainViewModel();
        }
    }
}