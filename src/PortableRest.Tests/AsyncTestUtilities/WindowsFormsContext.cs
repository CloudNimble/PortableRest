// from https://github.com/StephenCleary/AsyncCTPUtil/blob/master/AsyncTestUtilities/WindowsFormsContext.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortableRest.Tests.AsyncTestUtilities
{
    /// <summary>
    /// Async methods can run in a myriad of contexts - some have a "thread affinity"
    /// such that continuations are posted back in a way that ensures that they always
    /// execute on the originating thread.
    /// 
    /// Windows Forms is one of such contexts. ASP.NET is another.
    /// </summary>
    public static class WindowsFormsContext
    {
        /// <summary>
        /// Runs the function inside a message loop and continues pumping messages
        /// until the returned task completes. 
        /// </summary>
        /// <returns>The completed task returned by the delegate's invocation</returns>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            return ((Task<TResult>)Run((Func<Task>)function));
        }

        /// <summary>
        /// Runs the function inside a message loop and continues pumping messages
        /// until the returned task completes. 
        /// </summary>
        /// <returns>The completed task returned by the delegate's invocation</returns>
        public static Task Run(Func<Task> function)
        {
            using (InstallerAndRestorer.Install())
            {
                // InstallerAndRestorer ensures the WinForms context is installed
                var winFormsContext = SynchronizationContext.Current;

                var message = new TaskFunctionLaunchMessage(function, winFormsContext);

                // queue up our first message before we run the loop
                winFormsContext.Post(message.LaunchMessageImpl, null);

                // run the actual WinForms message loop
                Application.Run();

                if (message.ReturnedTask != null)
                {
                    message.ReturnedTask.RethrowForCompletedTasks();
                }

                return message.ReturnedTask;
            }
        }

        // a helper class to represent the initial message that
        // we post to the queue to invoke the delegate as well
        // as set up our plumbing to shut down the loop at the right time

        /// <summary>
        /// Runs the action inside a message loop and continues pumping messages
        /// as long as any asynchronous operations have been registered
        /// </summary>
        public static void Run(Action asyncAction)
        {
            using (InstallerAndRestorer.Install())
            {
                // InstallerAndRestorer ensures the WinForms context is installed
                // capture that WinFormsContext
                var winFormsContext = SynchronizationContext.Current;

                // wrap the WinForms context in our own decorator context and install that
                var asyncVoidContext = new AsyncVoidSyncContext(winFormsContext);
                SynchronizationContext.SetSynchronizationContext(asyncVoidContext);

                // queue up the first message before we start running the loop
                var message = new AsyncActionLaunchMessage(asyncAction, asyncVoidContext);
                asyncVoidContext.Post(message.LaunchMessageImpl, null);

                // run the actual WinForms message loop
                Application.Run();
            }
        }

        private static void RequestMessageLoopTermination(this SynchronizationContext syncContext)
        {
            syncContext.Post(state => Application.ExitThread(), null);
        }

        private class AsyncActionLaunchMessage
        {
            private readonly Action _asyncAction;
            private readonly AsyncVoidSyncContext _postingContext;

            public AsyncActionLaunchMessage(Action asyncAction, AsyncVoidSyncContext postingContext)
            {
                _asyncAction = asyncAction;
                _postingContext = postingContext;
            }

            // this signature is to match SendOrPostCallback
            public void LaunchMessageImpl(object ignoredState)
            {
                // now invoke our taskFunction and store the result
                // Do an explicit increment/decrement.
                // Our sync context does a check on decrement, to see if there are any
                // outstanding asynchronous operations (async void methods register this correctly).
                // If there aren't any registerd operations, then it will exit the loop
                _postingContext.OperationStarted();
                try
                {
                    _asyncAction.Invoke();
                }
                finally
                {
                    _postingContext.OperationCompleted();
                }
            }
        }

        private class AsyncVoidSyncContext : SynchronizationContext
        {
            private readonly SynchronizationContext _inner;
            private int _operationCount;

            /// <summary>Constructor for creating a new AsyncVoidSyncContext. Creates a new shared operation counter.</summary>
            public AsyncVoidSyncContext(SynchronizationContext innerContext)
            {
                _inner = innerContext;
            }

            public override SynchronizationContext CreateCopy()
            {
                return new AsyncVoidSyncContext(_inner.CreateCopy());
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _inner.Post(d, state);
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                _inner.Send(d, state);
            }

            public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
            {
                return _inner.Wait(waitHandles, waitAll, millisecondsTimeout);
            }

            public override void OperationStarted()
            {
                _inner.OperationStarted();
                Interlocked.Increment(ref _operationCount);
            }

            public override void OperationCompleted()
            {
                _inner.OperationCompleted();
                if (Interlocked.Decrement(ref _operationCount) == 0)
                {
                    this.RequestMessageLoopTermination();
                }
            }
        }

        private struct InstallerAndRestorer : IDisposable
        {
            private bool _originalAutoInstallValue;
            private SynchronizationContext _originalSyncContext;
            private Control _tempControl;

            public void Dispose()
            {
                // dispose our temporary control
                if (_tempControl != null)
                {
                    _tempControl.Dispose();
                    _tempControl = null;
                }

                // restore the autoinstall value
                WindowsFormsSynchronizationContext.AutoInstall = _originalAutoInstallValue;

                // restore the sync context
                SynchronizationContext.SetSynchronizationContext(_originalSyncContext);
            }

            public static InstallerAndRestorer Install()
            {
                // save the values to restore
                var iar = new InstallerAndRestorer
                {
                    _originalAutoInstallValue = WindowsFormsSynchronizationContext.AutoInstall,
                    _originalSyncContext = SynchronizationContext.Current
                };
                WindowsFormsSynchronizationContext.AutoInstall = true; // enable autoinstall of the official WinForms context
                iar._tempControl = new Control
                {
                    Visible = false
                }; // create a control, which will cause the WinForms context to become installed
                return iar;
            }
        }

        private class TaskFunctionLaunchMessage
        {
            private readonly SynchronizationContext _postingContext;
            private readonly Func<Task> _taskFunction;
            public Task ReturnedTask;

            public TaskFunctionLaunchMessage(Func<Task> taskFunction, SynchronizationContext postingContext)
            {
                _taskFunction = taskFunction;
                _postingContext = postingContext;
            }

            // this signature is to match SendOrPostCallback
            public void LaunchMessageImpl(object ignoredState)
            {
                // now invoke our taskFunction and store the returned task
                ReturnedTask = _taskFunction.Invoke();

                if (ReturnedTask != null)
                {
                    // register a continuation with the task, which will shut down the loop when the task completes.
                    ReturnedTask.ContinueWith(delegate { _postingContext.RequestMessageLoopTermination(); }, TaskContinuationOptions.ExecuteSynchronously);
                }
                else
                {
                    // the delegate returned a null Task (VB/C# compilers never do this for async methods)
                    // we don't have anything to register continuations with in this case, so exit out of the message loop
                    // immediately
                    Application.ExitThread();
                }
            }
        }
    }
}