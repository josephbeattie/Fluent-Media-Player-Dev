using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rise.Common.Helpers
{
    /// <summary>
    /// Represents a helper through which you can run multiple Tasks and prevent them from
    /// running concurrently. It seamlessly waits for any pending Tasks to be canceled or
    /// run to completion in a "Last In, Only Out" manner.
    /// </summary>
    public class CancellableTaskHelper
    {
        /// <summary>
        /// The task that is currently pending.
        /// </summary>
        protected Task _pendingTask = null;

        /// <summary>
        /// A linked token source to control Task execution.
        /// </summary>
        protected CancellationTokenSource _tokenSource = null;

        /// <summary>
        /// The current CancellationToken.
        /// </summary>
        public CancellationToken Token => _tokenSource.Token;

        /// <summary>
        /// Cancels the pending Task and waits for it to complete. This method
        /// must be called before <see cref="RunAsync(Task)"/> or any of its
        /// overloads.
        /// </summary>
        /// <returns>A Task that represents the pending task cancellation and
        /// the wait until completion.</returns>
        /// <exception cref="OperationCanceledException">If the new token has
        /// been canceled before the Task, an exception is thrown.</exception>
        public async Task CompletePendingAsync(CancellationToken token)
        {
            // Generate a new linked token
            var previousCts = this._tokenSource;
            var newCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            this._tokenSource = newCts;

            if (previousCts != null)
            {
                // Cancel the previous session and wait for its termination
                previousCts.Cancel();

                if (_pendingTask != null)
                    try { await this._pendingTask; } catch { }
            }

            // We need to check if we've been canceled
            newCts.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Runs a Task asynchronously. This Task will not run concurrently
        /// with any other Tasks ran through <see cref="RunAsync(Task)"/>
        /// or any of its overloads.
        /// </summary>
        /// <param name="task">Task to run.</param>
        /// <exception cref="OperationCanceledException">Thrown when the
        /// operation is cancelled.</exception>
        /// <remarks>For the helper to be effective, first call
        /// <see cref="CompletePendingAsync(CancellationToken)"/>, and pass
        /// <see cref="Token"/> to your task for the cancellation token.</remarks>
        public Task RunAsync(Task task)
        {
            this._pendingTask = task;
            return task;
        }

        /// <summary>
        /// Runs a Task asynchronously. This Task will not run concurrently
        /// with any other Tasks ran through <see cref="RunAsync(Task)"/>
        /// or any of its overloads.
        /// </summary>
        /// <param name="task">Task to run.</param>
        /// <returns>The return value of <paramref name="task"/>.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the
        /// operation is cancelled.</exception>
        /// <remarks>For the helper to be effective, first call
        /// <see cref="CompletePendingAsync(CancellationToken)"/>, and pass
        /// <see cref="Token"/> to your task for the cancellation token.</remarks>
        public Task<T> RunAsync<T>(Task<T> task)
        {
            this._pendingTask = task;
            return task;
        }
    }
}
