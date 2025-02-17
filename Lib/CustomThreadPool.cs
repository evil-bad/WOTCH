﻿using System;
using System.Threading;

namespace WOTCH.Lib
{
    public class CustomThreadPool
    {
        public delegate void OperationCompletedDelegate(bool isSuccess);
        public static event OperationCompletedDelegate OnOperationCompleted;

        public static void QueueUserWorkItem(WaitCallback operation, object state)
        {
            try
            {
                bool isSuccesfulyAdded = ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        operation(o);
                        OnOperationCompleted?.Invoke(true);
                    }
                    catch
                    {
                        OnOperationCompleted?.Invoke(false);
                    }
                }, state);

                if(!isSuccesfulyAdded)
                    OnOperationCompleted?.Invoke(false);
            }
            catch
            {
                OnOperationCompleted?.Invoke(false);
            }
        }
    }
}
