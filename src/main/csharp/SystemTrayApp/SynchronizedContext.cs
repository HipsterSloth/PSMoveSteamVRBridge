using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    // From: http://danderson.io/posts/using-isynchronizeinvoke-to-update-your-ui-safely-from-another-thread/
    /// <summary>
    /// Helper class that allows synchronized invoking to be performed in a single line of code.
    /// </summary>
    internal static class SynchronizedInvoke
    {
        /// <summary>
        /// Invokes the specified action on the thread that the specified sync object was created on.
        /// </summary>
        public static void Invoke(ISynchronizeInvoke sync, Action action)
        {
            if (!sync.InvokeRequired) {
                action();
            }
            else {
                object[] args = new object[] { };
                sync.Invoke(action, args);
            }
        }
    }

    public class SimpleAsyncResult : IAsyncResult
    {
        private object _state;
        public bool IsCompleted { get; set; }
        public WaitHandle AsyncWaitHandle { get; internal set; }

        public object AsyncState
        {
            get
            {
                if (Exception != null) {
                    throw Exception;
                }
                return _state;
            }
            internal set { _state = value; }
        }

        public bool CompletedSynchronously
        {
            get { return IsCompleted; }
        }

        internal Exception Exception { get; set; }
    }

    public class SynchronizedContext : ISynchronizeInvoke
    {
        private readonly object _sync = new object();

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            var result = new SimpleAsyncResult();

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    result.AsyncWaitHandle = new ManualResetEvent(false);

                    try {
                        result.AsyncState = Invoke(method, args);
                    }
                    catch (Exception exception) {
                        Debug.WriteLine(exception.Message);
                        Debug.WriteLine(exception.StackTrace);
                        result.Exception = exception;
                    }
                    result.IsCompleted = true;
                });


            return result;
        }

        public object EndInvoke(IAsyncResult result)
        {
            if (!result.IsCompleted) {
                result.AsyncWaitHandle.WaitOne();
            }

            return result.AsyncState;
        }

        public object Invoke(Delegate method, object[] args)
        {
            lock (_sync) {
                return method.DynamicInvoke(args);
            }
        }

        public bool InvokeRequired
        {
            get { return true; }
        }
    }
}
