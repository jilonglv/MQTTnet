
/******************************************************
* author :  jilonglv
* email  : jilonglv@gmail.com
* function:
* history:  created by jilonglv 2020/8/7 18:14:39
* clrversion :4.0.30319.42000
******************************************************/

namespace MQTTnet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskExtension
    {
        /// <summary>
        /// 兼容net40
        /// Task.FromResult
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="v">返回数据</param>
        /// <returns></returns>
        public static Task<T> FromResult<T>(T v)
        {
#if NET40
            return Task.Factory.StartNew<T>(() => v);
#else
            return Task.FromResult(v);
#endif
        }
        public static Task Run(Action action)
        {
#if NET40
            return Task.Factory.StartNew(action);
#else
            return Task.Run(action);
#endif
        }
        public static Task Run(Action action,CancellationToken cancellationToken)
        {
#if NET40
            return Task.Factory.StartNew(action,cancellationToken);
#else
            return Task.Run(action,cancellationToken);
#endif
        }
        public static Task Delay(int milliseconds, CancellationToken cancellationToken)
        {
#if NET40
            return Run(() => Thread.Sleep(milliseconds),cancellationToken);
#else
            return Task.Delay(milliseconds,cancellationToken);
#endif
        }
        public static Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
#if NET40
            return Run(() => Thread.Sleep(timeSpan), cancellationToken);
#else
            return Task.Delay(timeSpan,cancellationToken);
#endif
        }
#if NET40
        /// <summary>
        /// 异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        //public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, int timeout, System.Threading.CancellationTokenSource timeoutCancellationTokenSource)
        //{
        //    //using (var timeoutCancellationTokenSource = new System.Threading.CancellationTokenSource())
        //    {
        //        var delayTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
        //        if (await Task.WhenAny(task, delayTask) == task)
        //        {
        //            timeoutCancellationTokenSource.Cancel();
        //            return await task;
        //        }
        //        throw new TimeoutException("The operation has timed out.");
        //    }
        //}
        ///// <summary>
        ///// 异步
        ///// </summary>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="task"></param>
        ///// <param name="timeout"></param>
        ///// <param name="cancelToken"></param>
        ///// <returns></returns>
        //public static async Task WaitAsync(this Task task, int timeout, System.Threading.CancellationTokenSource timeoutCancellationTokenSource)
        //{
        //    //using (var timeoutCancellationTokenSource = new System.Threading.CancellationTokenSource())
        //    {
        //        var delayTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
        //        if (await Task.WhenAny(task, delayTask) == task)
        //        {
        //            timeoutCancellationTokenSource.Cancel();
        //            await task;
        //        }
        //        throw new TimeoutException("The operation has timed out.");
        //    }
        //}
#endif
        //public static Task<T> FromResult<T>(T v)
        //{
        //    return Task.Factory.StartNew<T>(() => v);
        //}
    }
}
