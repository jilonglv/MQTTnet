using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Exceptions;

namespace MQTTnet.Internal
{
    public static class MqttTaskTimeout
    {
        public static async Task WaitAsync(Func<CancellationToken, Task> action, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
#if NET40
            using (var timeoutCts = new CancellationTokenSource())
            {
                timeoutCts.CancelAfter(timeout);
#else
            using (var timeoutCts = new CancellationTokenSource(timeout))
            {
#endif
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {
                    try
                    {
                        await action(linkedCts.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException exception)
                    {
                        var timeoutReached = timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested;
                        if (timeoutReached)
                        {
                            throw new MqttCommunicationTimedOutException(exception);
                        }

                        throw;
                    }
                }
            }
        }

        public static async Task<TResult> WaitAsync<TResult>(Func<CancellationToken, Task<TResult>> action, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
#if NET40
            using (var timeoutCts = new CancellationTokenSource())
            {
                timeoutCts.CancelAfter(timeout);
#else
            using (var timeoutCts = new CancellationTokenSource(timeout))
            {
#endif
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {
                    try
                    {
                        return await action(linkedCts.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException exception)
                    {
                        var timeoutReached = timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested;
                        if (timeoutReached)
                        {
                            throw new MqttCommunicationTimedOutException(exception);
                        }

                        throw;
                    }
                }
            }
        }     
    }
}
