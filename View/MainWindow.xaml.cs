using System.Windows;
using ThongPanelFrame.ViewModel;

namespace ThongPanelFrame.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowVM();
            //DispatcherHelper.Initialize();
        }
    }
}