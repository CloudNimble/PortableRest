// from https://github.com/StephenCleary/AsyncCTPUtil/blob/master/AsyncTestUtilities/AsyncTestUtilities.cs

using System.Threading.Tasks;

namespace PortableRest.Tests.AsyncTestUtilities
{

    /// <summary>
    /// 
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 'await' has semantics so that if you await a task, it will
        /// throw if the task did not run to completion normally (similar to synchronous code).
        /// 
        /// This extension method allows for you to invoke this for completed tasks (a task in
        /// the RanToCompletion, Cancelled, or Faulted state).
        /// 
        /// Thus, if the task was cancelled or encountered an exception during
        /// execution, this will now trigger that exception to be propogated.
        /// </summary>
        public static void RethrowForCompletedTasks(this Task task)
        {
            // Here we do a bit of trickery. We explicitly call the methods that are underlying
            // the await pattern so that we so that we can take advantage of the same Task exception
            // packaging/unpackaging mechanisms.
            //
            // This doesn't actually do a true 'await', but emulates the await pattern.
            // For tasks that haven't completed, it throws an exception rather than
            // signs up the rest of the method as a continuation.
            //
            // The pattern contract is that GetResult() can be called synchronously if IsCompleted returned true.

            task.GetAwaiter().GetResult();
        }

    }

}