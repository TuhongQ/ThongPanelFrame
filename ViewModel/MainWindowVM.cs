using ThongPanelFrame.Base;
using ThongPanelFrame.Model;
using ThongPanelFrame.Model.Recipe;
using ThongPanelFrame.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;

namespace ThongPanelFrame.ViewModel
{
    public class RecipeList : ViewModelBase
    {
        private string _strResult;
        public string num { get; set; }
        public string testDescription { get; set; }

        public string strResult
        {
            get { return _strResult; }
            set
            {
                _strResult = value;
                RaisePropertyChanged(nameof(strResult));
            }
        }
    }

    public class MainWindowVM : ViewModelBase
    {
        private XThread _threadProcessMonitor;
        private CMDModel? _strCMDM;
        private ObservableCollection<RecipeList> _recipeLists;

        //public ObservableCollection<RecipeList> RecipeLists { get => _recipeLists; set => Set(ref _recipeLists, value); }
        public ObservableCollection<RecipeList> RecipeLists
        {
            get { return _recipeLists; }
            set
            {
                _recipeLists = value;
                RaisePropertyChanged(nameof(RecipeLists));
            }
        }

        public CMDModel? StrCMDM
        {
            get { return _strCMDM; }
            set
            {
                _strCMDM = value;
                RaisePropertyChanged(nameof(StrCMDM));
            }
        }

        public MainWindowVM()
        {
            RecipeLists = new ObservableCollection<RecipeList>();

            Global.bUpdataList = true;

            StrCMDM = new CMDModel();
            _threadProcessMonitor = new XThread(ProcessMonitor);
            _threadProcessMonitor.Start();
        }

        private void UpdataList()
        {
            RecipeLists.Clear();
            for (int i = 0; i < RecipeManager.Instance.Data.ADBParams.Count; i++)
            {
                RecipeLists.Add(new RecipeList { num = i.ToString(), testDescription = RecipeManager.Instance.Data.ADBParams[i].Description, strResult = "wait" });
                Global.AllResult.Add("wait");
            }
            Global.bUpdataList = false;
        }

        private void PartUpdate()
        {
        }

        public ICommand IsStart
        {
            get { return new RelayCommand<object>(Start); }
        }

        public ICommand IsStop
        {
            get { return new RelayCommand<object>((object parame) => { Machine.Instance.StopAuto(); }); }
        }

        public ICommand OpenProperty
        {
            get { return new RelayCommand<object>(OpenPropety); }
        }

        private void OpenPropety(object parameter)
        {
            //var windows = new Window();
            //PropertyGrid pro=new PropertyGrid ();
            //windows.Content= pro;
            //windows.Show();

            PropertyGrid pro = new PropertyGrid();
            pro.Show();
        }

        private void Start(object parameter)
        {
            Machine.Instance.StartAuto();
        }

        /// <summary>
        /// 界面状态刷新
        /// </summary>
        /// <param name="parame"></param>
        private async void ProcessMonitor(object parame)
        {
            XThread? thread = parame as XThread;
            while (true)
            {
                if (thread == null || thread.BlockWaitEvent()) { break; }
                await DispatcherHelper.RunAsync(() =>
                {
                    //StrCMDM.strResult = Global.AllResult;
                    StrCMDM.strDescription = Global.GlADBDescription;
                    StrCMDM = StrCMDM;
                    if (Global.bUpdataList)
                    {
                        UpdataList();
                    }
                    else
                    {
                        for (int i = 0; i < RecipeLists.Count; i++)
                        {
                            RecipeLists[i].strResult = Global.AllResult[i];
                        }
                    }
                });
                Thread.Sleep(300);
            }
        }
    }
}