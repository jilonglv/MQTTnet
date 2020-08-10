using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTnet.Internal
{
    public sealed class AsyncQueue<TItem> : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        private ConcurrentQueue<TItem> _queue = new ConcurrentQueue<TItem>();

        public int Count => _queue.Count;

        public void Enqueue(TItem item)
        {
            _queue.Enqueue(item);
            _semaphore.Release();
        }
#pragma warning disable 1998
        public async Task<AsyncQueueDequeueResult<TItem>> TryDequeueAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
#if NET40
                    _semaphore.Wait(cancellationToken);
#else
                    await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
#endif
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    return new AsyncQueueDequeueResult<TItem>(false, default(TItem));
                }

                if (_queue.TryDequeue(out var item))
                {
                    return new AsyncQueueDequeueResult<TItem>(true, item);
                }
            }

            return new AsyncQueueDequeueResult<TItem>(false, default(TItem));
        }
#pragma warning restore 1998
        public AsyncQueueDequeueResult<TItem> TryDequeue()
        {
            if (_queue.TryDequeue(out var item))
            {
                return new AsyncQueueDequeueResult<TItem>(true, item);
            }

            return new AsyncQueueDequeueResult<TItem>(false, default(TItem));
        }

        public void Clear()
        {
            Interlocked.Exchange(ref _queue, new ConcurrentQueue<TItem>());
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}
