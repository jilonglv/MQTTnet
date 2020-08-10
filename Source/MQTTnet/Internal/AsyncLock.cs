using System;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTnet.Internal
{
    // From Stephen Toub (https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-6-asynclock/)
    public sealed class AsyncLock : IDisposable
    {
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        readonly Task<IDisposable> _releaser;

        public AsyncLock()
        {
            _releaser = TaskExtension.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> WaitAsync()
        {
            return WaitAsync(CancellationToken.None);
        }

        public Task<IDisposable> WaitAsync(CancellationToken cancellationToken)
        {
#if NET40
            _semaphore.Wait();
            return TaskExtension.FromResult(_releaser.Result);
#else
            var task = _semaphore.WaitAsync(cancellationToken);
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return _releaser;
            }

            return task.ContinueWith(
                (_, state) => (IDisposable)state, 
                _releaser.Result, 
                cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
#endif
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }

        class Releaser : IDisposable
        {
            readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            public void Dispose()
            {
                _toRelease._semaphore.Release();
            }
        }
    }
}
