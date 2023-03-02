using ThongPanelFrame.Base;
using System;
using System.Threading;

namespace ThongPanelFrame.Model.Process
{
    public class ProcAuto
    {
        //执行自动化流程
        private XThread _threadProc;

        //工位线程，同时执行多工位，备用
        private XThread[] _threadProcStages;

        ProcStage procStage = new ProcStage();//可能要用单例，先这样写
        public ProcAuto()
        {
            _threadProc = new XThread(Process);
        }
        public void Process(object parame)
        {
            XThread thread = parame as XThread;
            int iNum = 1;
            while (true)
            {
                if (thread==null|| thread.BlockWaitEvent()) break;              
                switch (iNum)
                {
                    case 1:
                        procStage.Load();
                        iNum= 2;
                        break;
                    case 2:
                        break;
                    default:
                        throw new Exception($"未定义类型");
                }
            }
        }
        public void Start()
        {
            _threadProc.Start();

            //for (int i = 0; i < ProcStages.Length; i++)
            //    _threadProcStages[i].Start();
        }

        public void Resume()
        {
            _threadProc.Resume();

            //for (int i = 0; i < ProcStages.Length; i++)
            //    _threadProcStages[i].Resume();
        }

        public void Pause()
        {
            _threadProc.Pause();

            //for (int i = 0; i < ProcStages.Length; i++)
            //    _threadProcStages[i].Pause();
        }

        public void Stop()
        {
            _threadProc.Stop();

            //for (int i = 0; i < ProcStages.Length; i++)
            //    _threadProcStages[i].Stop();
        }

    }
}