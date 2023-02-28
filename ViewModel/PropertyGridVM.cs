using ThongPanelFrame.Model;
using ThongPanelFrame.Model.Recipe;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ThongPanelFrame.ViewModel
{
    public class PropertyGridVM
    {
        public RecipeParam SelectedValue { get; set; }


        public PropertyGridVM()
        {
            RecipeManager.Instance.Load();//属性修改后会导致 RecipeManager.Instance.Data值变化，故重新加载xml
            SelectedValue = RecipeManager.Instance.Data;

        }
        public ICommand IsSave
        {
            get { return new RelayCommand<object>(Save); }
        }
        public void Save(object parameter)
        {
            Button btnSave =  parameter as Button;
            if (btnSave != null) { btnSave.Background = Brushes.Red; }
            RecipeManager.Instance.Save(SelectedValue.Name.ToString());
            Global.bUpdataList = true;
        }
    }
}
