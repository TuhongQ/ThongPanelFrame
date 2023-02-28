using System.Diagnostics;
using System.Threading;

namespace ThongPanelFrame.Base
{
    public enum XThreadState
    {
        Run,
        Pause,
        Stop
    }

    public class XThread
    {
        #region Thread State

        private XThreadState _state;

        public XThreadState State
        {
            get
            {
                if (_thread == null || !_thread.IsAlive)
                {
                    _state = XThreadState.Stop;
                }
                return _state;
            }
        }

        #endregion Thread State

        private Thread? _thread;
        private ParameterizedThreadStart _threadContent;

        private readonly ManualResetEvent _eventPause = new ManualResetEvent(false);
        private readonly ManualResetEvent _eventStop = new ManualResetEvent(false);

        public XThread(ParameterizedThreadStart content)
        {
            _state = XThreadState.Stop;
            _threadContent = content;
        }

        /// <summary>
        /// 线程堵塞等待事件，
        /// </summary>
        /// <returns></returns>
        public bool BlockWaitEvent()
        {
            if (_eventStop.WaitOne(0))
                return true;

            if (_eventPause.WaitOne(0))
            {
                XThreadState state = _state;
                _state = XThreadState.Pause;

                while (_eventPause.WaitOne(0))
                {
                    if (_eventStop.WaitOne(0))
                    {
                        _state = state;
                        return true;
                    }
                    Thread.Sleep(10);
                }
                _state = state;
            }
            return false;
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (_thread != null && _thread.IsAlive)
                return false;

            _eventPause.Reset();
            _eventStop.Reset();

            _thread = new Thread(_threadContent);
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.IsBackground = true;
            _thread.Start(this);

            _state = XThreadState.Run;

            return true;
        }

        /// <summary>
        /// 暂停线程
        /// </summary>
        /// <param name="isWait">是否等待执行完成</param>
        /// <param name="millisecondsTimeout">等待超时时间</param>
        /// <returns></returns>
        public bool Pause(bool isWait = true, int millisecondsTimeout = 30000)
        {
            if (_thread != null && _thread.IsAlive)
            {
                if (_state == XThreadState.Run)
                {
                    _eventPause.Set();

                    if (isWait)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        while (_state != XThreadState.Pause)
                        {
                            if (stopwatch.ElapsedMilliseconds > millisecondsTimeout)
                            {
                                return false;
                            }

                            Thread.Sleep(1);
                        }
                    }

                    return true;
                }
                else if (_state == XThreadState.Pause)
                {
                    _eventPause.Set();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 继续线程
        /// </summary>
        /// <returns></returns>
        public bool Resume()
        {
            if (_thread != null && _thread.IsAlive)
            {
                if (_state == XThreadState.Run || _state == XThreadState.Pause)
                {
                    _eventPause.Reset();
                    _state = XThreadState.Run;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Thread Get_thread()
        {
            return _thread;
        }

        /// <summary>
        /// 结束线程。在正常退出线程超时后，将强行调用Abort()终止线程。
        /// </summary>
        /// <param name="isWait">是否等待执行完成</param>
        /// <param name="millisecondsTimeout">等待超时时间</param>
        /// <returns></returns>
        public bool Stop(bool isWait = true, int millisecondsTimeout = 30000)
        {
            if (_thread == null || !_thread.IsAlive)
            {
                _state = XThreadState.Stop;
                return true;
            }

            _eventStop.Set();

            if (isWait)
            {
                //_thread.Join(millisecondsTimeout);

                Stopwatch stopwatch = Stopwatch.StartNew();
                while (_thread.IsAlive)
                {
                    if (stopwatch.ElapsedMilliseconds > millisecondsTimeout) break;

                    Thread.Sleep(1);
                }

                if (!_thread.IsAlive)
                {
                    _thread = null;
                    _state = XThreadState.Stop;
                    return true;
                }
                else
                {
                    _thread.Abort();
                    _thread.Join();
                    _thread = null;
                    _state = XThreadState.Stop;
                    //throw new Exception("线程退出超时！");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}