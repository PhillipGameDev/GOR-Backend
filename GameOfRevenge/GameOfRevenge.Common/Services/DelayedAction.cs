using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Services
{
    public class DelayedAction
    {
        public bool isDispose;

        public async void WaitForCallBack(Action action, int delay)
        {
            await Task.Delay(delay);
            if (isDispose == false) action?.Invoke();
        }

        public async void WaitForCallBack<T>(Action<T> action, int delay, T parameter)
        {
            await Task.Delay(delay);
            if (isDispose == false) action?.Invoke(parameter);
        }

        public async void WaitForCallBack<T1, T2>(Action<T1, T2> action, int delay, T1 parameter1, T2 parameter2)
        {
            await Task.Delay(delay);
            if (isDispose == false) action?.Invoke(parameter1, parameter2);
        }

        public async void WaitForCallBack<T1, T2, T3>(Action<T1, T2, T3> action, int delay, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            await Task.Delay(delay);
            if (isDispose == false) action?.Invoke(parameter1, parameter2, parameter3);
        }

        public async void WaitForCallBack<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, int delay, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
        {
            await Task.Delay(delay);
            if (isDispose == false) action?.Invoke(parameter1, parameter2, parameter3, parameter4);
        }

        public async void WaitForCallBack<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, int delay, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5)
        {
            await Task.Delay(delay);
            if (isDispose == false) action?.Invoke(parameter1, parameter2, parameter3, parameter4, parameter5);
        }

        public async void WaitForCallBack<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, int delay, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6)
        {
            await Task.Delay(delay);
            if (isDispose == false) action?.Invoke(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6);
        }

        public void Dispose()
        {
            isDispose = true;
        }
    }

}
