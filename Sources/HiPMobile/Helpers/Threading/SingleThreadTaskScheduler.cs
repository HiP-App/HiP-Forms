using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Threading
{
    /// <summary>
    ///     Represents a <see cref="TaskScheduler"/> which executes code on a dedicated, single thread whose <see cref="ApartmentState"/> can be configured.
    /// </summary>
    /// <remarks>
    ///     You can use this class if you want to perform operations on a non thread-safe library from a multi-threaded environment.
    /// </remarks>
    public sealed class SingleThreadTaskScheduler
        : TaskScheduler, IDisposable
    {
        private readonly Thread thread;
        private readonly CancellationTokenSource cancellationToken;
        private readonly BlockingCollection<Task> tasks;
        private readonly Action initAction;

        /// <summary>
        ///     The <see cref="System.Threading.ApartmentState"/> of the <see cref="Thread"/> this <see cref="SingleThreadTaskScheduler"/> uses to execute its work.
        /// </summary>
        public ApartmentState ApartmentState { get; private set; }

        /// <summary>
        ///     Indicates the maximum concurrency level this <see cref="T:System.Threading.Tasks.TaskScheduler"/> is able to support.
        /// </summary>
        /// 
        /// <returns>
        ///     Returns <c>1</c>.
        /// </returns>
        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleThreadTaskScheduler"/>, optionally setting an <see cref="System.Threading.ApartmentState"/>.
        /// </summary>
        /// <param name="apartmentState">
        ///     The <see cref="ApartmentState"/> to use. Defaults to <see cref="System.Threading.ApartmentState.STA"/>
        /// </param>
        public SingleThreadTaskScheduler(ApartmentState apartmentState = ApartmentState.STA)
            : this(null, apartmentState)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleThreadTaskScheduler"/> passsing an initialization action, optionally setting an <see cref="System.Threading.ApartmentState"/>.
        /// </summary>
        /// <param name="initAction">
        ///     An <see cref="Action"/> to perform in the context of the <see cref="Thread"/> this <see cref="SingleThreadTaskScheduler"/> uses to execute its work after it has been started.
        /// </param>
        /// <param name="apartmentState">
        ///     The <see cref="ApartmentState"/> to use. Defaults to <see cref="System.Threading.ApartmentState.STA"/>
        /// </param>
        public SingleThreadTaskScheduler(Action initAction, ApartmentState apartmentState = ApartmentState.STA)
        {
            if (apartmentState != ApartmentState.MTA && apartmentState != ApartmentState.STA)
                throw new ArgumentException("apartementState");

            ApartmentState = apartmentState;
            cancellationToken = new CancellationTokenSource();
            tasks = new BlockingCollection<Task>();
            this.initAction = initAction ?? (() => { });

            thread = new Thread(ThreadStart);
            thread.IsBackground = true;
            thread.TrySetApartmentState(apartmentState);
            thread.Start();
        }

        /// <summary>
        ///     Waits until all scheduled <see cref="Task"/>s on this <see cref="SingleThreadTaskScheduler"/> have executed and then disposes this <see cref="SingleThreadTaskScheduler"/>.
        /// </summary>
        /// <remarks>
        ///     Calling this method will block execution. It should only be called once.
        /// </remarks>
        /// <exception cref="TaskSchedulerException">
        ///     Thrown when this <see cref="SingleThreadTaskScheduler"/> already has been disposed by calling either <see cref="Wait"/> or <see cref="Dispose"/>.
        /// </exception>
        public void Wait()
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskSchedulerException("Cannot wait after disposal.");

            tasks.CompleteAdding();
            thread.Join();

            cancellationToken.Cancel();
        }

        /// <summary>
        ///     Disposes this <see cref="SingleThreadTaskScheduler"/> by not accepting any further work and not executing previously scheduled tasks.
        /// </summary>
        /// <remarks>
        ///     Call <see cref="Wait"/> instead to finish all queued work. You do not need to call <see cref="Dispose"/> after calling <see cref="Wait"/>.
        /// </remarks>
        public void Dispose()
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            tasks.CompleteAdding();
            cancellationToken.Cancel();
        }

        protected override void QueueTask(Task task)
        {
            VerifyNotDisposed();

            tasks.Add(task, cancellationToken.Token);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            VerifyNotDisposed();

            if (thread != Thread.CurrentThread)
                return false;
            if (cancellationToken.Token.IsCancellationRequested)
                return false;

            TryExecuteTask(task);
            return true;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            VerifyNotDisposed();

            return tasks.ToArray();
        }

        private void ThreadStart()
        {
            try
            {
                var token = cancellationToken.Token;

                initAction();

                foreach (var task in tasks.GetConsumingEnumerable(token))
                    TryExecuteTask(task);
            }
            finally
            {
                tasks.Dispose();
            }
        }

        private void VerifyNotDisposed()
        {
            if (cancellationToken.IsCancellationRequested)
                throw new ObjectDisposedException(typeof(SingleThreadTaskScheduler).Name);
        }
    }
}