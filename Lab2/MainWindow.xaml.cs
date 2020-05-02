using Lab2.NorthWnd;
using System.Windows;


namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Northwnd = new NorthwndViewModel();
            DataContext = Northwnd;
        }
        public NorthwndViewModel Northwnd { get; private set; }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.Count > 0)
                Northwnd.SelectedOrder = e.AddedItems[0] as Order;
        }

    }
}
