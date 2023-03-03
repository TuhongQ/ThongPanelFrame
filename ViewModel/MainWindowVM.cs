using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ThongPanelFrame.ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        private Visibility _ishomepageVis = Visibility.Visible;

        public Visibility IshomepageVis
        {
            get { return _ishomepageVis; }
            set { _ishomepageVis = value; RaisePropertyChanged("IshomepageVis"); }
        }

        private Visibility _ispropertyVis = Visibility.Hidden;

        public Visibility IspropertyVis
        {
            get { return _ispropertyVis; }
            set { _ispropertyVis = value; RaisePropertyChanged(nameof(IspropertyVis)); }
        }

        public ICommand ChangePage
        {
            get { return new RelayCommand<object>(CurrentPage); }
        }

        private void CurrentPage(object paramer)
        {
            IshomepageVis = Visibility.Hidden;
            IspropertyVis = Visibility.Hidden;
            Button? menu = paramer as Button;
            switch (menu.Name)
            {
                case "HomeView":
                    IshomepageVis = Visibility.Visible;
                    break;
                case "ManageView":
                    break;
                case "RecipeView":
                    IspropertyVis = Visibility.Visible;
                    break;
                case "ProductLogView":
                    break;
                case "AlarmView":
                    break;
                case "SysSetView":
                    break;

                default:
                    break;
            }
        }
    }
}