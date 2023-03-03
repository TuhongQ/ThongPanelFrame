using System.Windows.Controls;
using ThongPanelFrame.ViewModel;

namespace ThongPanelFrame.View
{
    /// <summary>
    /// PropertyGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyGrid : UserControl
    {
        public PropertyGrid()
        {
            InitializeComponent();
            this.DataContext = new PropertyGridVM();
        }
    }
}