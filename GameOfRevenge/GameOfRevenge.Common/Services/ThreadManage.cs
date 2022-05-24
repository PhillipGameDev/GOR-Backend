using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Services
{
    public class ThreadManage
    {
        public Thread BigStackThread { get; set; }

        public void LaunchThread(Action action)
        {
            BigStackThread = new Thread(() => action(), 1024 * 1024);
            ThreadStart(BigStackThread);
        }
        public void LaunchThread<T>(Action<T> action, T param)
        {
            BigStackThread = new Thread(() => action(param), 1024 * 1024);
            ThreadStart(BigStackThread);
        }
        public void LaunchThread<T1, T2>(Action<T1, T2> action, T1 parameter1, T2 parameter2)
        {
            BigStackThread = new Thread(() => action(parameter1, parameter2), 1024 * 1024);
            ThreadStart(BigStackThread);

        }
        public void LaunchThread<T1, T2, T3>(Action<T1, T2, T3> action, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            BigStackThread = new Thread(() => action(parameter1, parameter2, parameter3), 1024 * 1024);
            ThreadStart(BigStackThread);
        }

        public void LaunchThread<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
        {
            BigStackThread = new Thread(() => action(parameter1, parameter2, parameter3, parameter4), 1024 * 1024);
            ThreadStart(BigStackThread);
        }

        public void LaunchThread<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parm5)
        {
            BigStackThread = new Thread(() => action(parameter1, parameter2, parameter3, parameter4, parm5), 1024 * 1024);
            ThreadStart(BigStackThread);
        }
        public void LaunchThread<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parm5, T6 param6)
        {
            BigStackThread = new Thread(() => action(parameter1, parameter2, parameter3, parameter4, parm5, param6), 1024 * 1024);
            ThreadStart(BigStackThread);
        }
        private void ThreadStart(Thread bigStackThread)
        {
            bigStackThread.IsBackground = true;
            bigStackThread.Start();
        }
        public void LaunchThreadWithJoin<T1, T2, T3>(Action<T1, T2, T3> action, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            BigStackThread = new Thread(() => action(parameter1, parameter2, parameter3), 1024 * 1024);
            ThreadStartWithJoin(BigStackThread);
        }
        private void ThreadStartWithJoin(Thread bigStackThread)
        {
            bigStackThread.IsBackground = true;
            bigStackThread.Start();
            bigStackThread.Join();
        }
    }


}
