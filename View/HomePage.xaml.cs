using GalaSoft.MvvmLight.Threading;
using System.Windows.Controls;
using ThongPanelFrame.ViewModel;

namespace ThongPanelFrame.View
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
            this.DataContext = new HomePageVM();
            DispatcherHelper.Initialize();
        }
    }
}