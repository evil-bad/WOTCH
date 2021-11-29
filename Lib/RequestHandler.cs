using System.Collections.Concurrent;
using WOTCH.Interfaces;

namespace WOTCH.Lib
{
    public class RequestHandler : IRequestHandler
    {
        private static readonly ConcurrentQueue<object> requestQueue = new();
        private static int restConcurrentCalls = DataProcessor.DataProcessor.MaxConcurrentCalls;
        private static readonly object processLock = new();
        private static readonly object operationCompleteLock = new();

        // must be singleton to prevent multiple event call
        // or implement explicit add/remove
        public RequestHandler()
        {
            CustomThreadPool.OnOperationCompleted += CustomThreadPool_OnOperationCompleted;
        }

        public void Proceed(object data)
        {
            lock (processLock)
            {
                restConcurrentCalls--;

                System.Diagnostics.Debug.WriteLine($"Started. Rest calls: {restConcurrentCalls}");

                if (restConcurrentCalls < 0)
                {
                    requestQueue.Enqueue(data);
                    return;
                }
            }

            CustomThreadPool.QueueUserWorkItem(state => DataProcessor.DataProcessor.ProcessData(data), null);
        }

        private void CustomThreadPool_OnOperationCompleted(bool isSuccess)
        {
            if(!isSuccess)
                System.Diagnostics.Debug.WriteLine($"Failed");

            lock (operationCompleteLock)
            {
                restConcurrentCalls++;

                System.Diagnostics.Debug.WriteLine($"Completed. Rest calls: {restConcurrentCalls}");

                if (requestQueue.IsEmpty) return;
            }

            if (requestQueue.TryDequeue(out object data))
                CustomThreadPool.QueueUserWorkItem(state => DataProcessor.DataProcessor.ProcessData(data), null);
        }
    }


    //public static class DataProcessor
    //{
    //    public static void ProcessData(object data)
    //    {
    //        Thread.Sleep(new Random().Next(100, 10000));
    //    }

    //    public static int MaxConcurrentCalls { get => 5; }
    //}
}
