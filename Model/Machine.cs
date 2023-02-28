using ThongPanelFrame.Base;
using ThongPanelFrame.Model.Process;
using System;

namespace ThongPanelFrame.Model
{
    public enum MachineStatus
    {
        None = 0,
        NotAvailable,
        Initialize,
        Ready,
        AutoRun,
        AutoPause,
        AutoIdle
    }

    public class Machine
    {
        private readonly object _lockOperate = new object();
        public MachineStatus MachineStatus { get; private set; } = MachineStatus.None;
        public ProcAuto ProcAuto { get; private set; }
        private static readonly Lazy<Machine> _singer = new Lazy<Machine>(() => new Machine());
        public static Machine Instance
        { get { return _singer.Value; } }

        private Machine()
        {
            ProcAuto = new ProcAuto();
        }

        public void StartAuto()
        {
            XThread thread = new XThread((param) =>
            {
                lock (_lockOperate)
                {
                    //执行方法
                    ProcAuto.Start();
                    MachineStatus = MachineStatus.AutoRun;
                }
            });
            thread.Start();
        }

        public void StopAuto()
        {
            XThread thread = new XThread((param) =>
            {
                lock (_lockOperate)
                {
                    ProcAuto.Stop();
                    MachineStatus = MachineStatus.Ready;
                }
            });
            thread.Start();
        }

        public void Pause()
        {
            XThread thread = new XThread((param) =>
            {
                lock (_lockOperate)
                {
                    ProcAuto.Pause();
                    MachineStatus = MachineStatus.AutoPause;
                }
            });
            thread.Start();
        }

        public void Resume()
        {
            XThread threadthread = new XThread((param) =>
            {
                lock (_lockOperate)
                {
                    ProcAuto.Resume();
                    MachineStatus = MachineStatus.AutoRun;
                }
            });
        }
    }
}