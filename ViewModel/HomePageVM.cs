using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using ThongPanelFrame.Base;
using ThongPanelFrame.Model;
using ThongPanelFrame.Model.Recipe;

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

    public class HomePageVM : ViewModelBase
    {
        private XThread _threadProcessMonitor;
        private int _progressBarValue=40;
        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                RaisePropertyChanged(nameof(ProgressBarValue));
            }
        }
        private string _machineStatus="Ready";
        public string MachineStatus
        {
            get { return _machineStatus; }
            set { _machineStatus = value;RaisePropertyChanged(nameof(MachineStatus)); }
        }
        private bool _btnStartEnable = true;
        public bool BtnStartEnabled
        {
            get { return _btnStartEnable; }
            set { _btnStartEnable = value; RaisePropertyChanged(nameof(BtnStartEnabled)); }
        }
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

        public HomePageVM()
        {
            RecipeLists = new ObservableCollection<RecipeList>();

            Global.bUpdataList = true;
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
        public ICommand IsPasue
        {
            get { return new RelayCommand<object>((object parame) => { Machine.Instance.Pause(); }); }
        }
        public ICommand IsStop
        {
            
            get { return new RelayCommand<object>(Stop); }
        }

        private void Start(object parameter)
        {
            //Button statrBtn = parameter as Button;
            //statrBtn.IsEnabled = false;
            BtnStartEnabled = false;
            Machine.Instance.StartAuto();
            
        }
        private void Stop(object parameter)
        {
            
            Machine.Instance.StopAuto();
            BtnStartEnabled = true;
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
                       ProgressBarValue= (int)Machine.Instance.MachineProcess;
                       MachineStatus = Machine.Instance.MachineStatus.ToString();
                       
                    }
                });
                Thread.Sleep(100);
            }
        }
    }
}