// Copyright (c) Adgistics Limited and others. All rights reserved. 
// The contents of this file are subject to the terms of the 
// Adgistics Development and Distribution License (the "License"). 
// You may not use this file except in compliance with the License.
// 
// http://www.adgistics.com/license.html
// 
// See the License for the specific language governing permissions
// and limitations under the License.
namespace AdgisticsMotorsReport.Utils.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    ///   <para>
    ///     This class allows for work to be queued and then processed via background worker threads.
    ///     The number of worker threads are configurable.  Worker threads process <see cref="IWork"/> items.
    ///   </para>
    /// </summary>
    public sealed class BackgroundWorkerQueue : IDisposable
    {
        /// <summary>
        /// Event handlers for when an <see cref="IWork"/> item fails during it's processing.
        /// </summary>
        /// <remarks>
        /// You can attach you own event listeners allowing response to failures.
        /// </remarks>
        public event EventHandler<WorkFailedProcessingEventArgs> WorkFailed;

        /// <summary>
        /// Event handlers for when an <see cref="IWork"/> item is successfully processed
        /// </summary>
        /// <remarks>
        /// You can attach you own event listeners allowing response to successfully processed items.
        /// </remarks>
        public event EventHandler<WorkSucceededProcessingEventArgs> WorkSucceeded;
        
        /// <summary>
        /// Lock object for accessing/altering the queue.
        /// </summary>
        private readonly object queueLocker = new object();

        /// <summary>
        /// Worker threads to process the <see cref="IWork"/> queued items.
        /// </summary>
        private readonly Thread[] workers;

        /// <summary>
        /// Work queue.
        /// </summary>
        private readonly Queue<IWork> queue = new Queue<IWork>();

        /// <summary>
        /// The items currently being processed.
        /// </summary>
        private readonly List<IWork> processing = new List<IWork>();

        /// <summary>
        /// The items that failed to be processed
        /// </summary>
        private readonly List<IWork> failed = new List<IWork>();

        /// <summary>
        /// Poison Pill used to shut down the workers gracefully.
        /// </summary>
        private static readonly IWork POISON_PILL = new PoisonPill();

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundWorkerQueue"/> class.
        /// </summary>
        /// <param name="workerThreads">The number worker threads.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="workerThreads"/> is &lt; 1.
        /// If <paramref name="workerThreads"/> is &gt; 100.
        /// </exception>
        public BackgroundWorkerQueue(int workerThreads)
        {
            if (workerThreads < 1) 
                throw new ArgumentException("Argument: 'workerThreads' must be > 0.");
            if (workerThreads >= 100)
                throw new ArgumentException("Argument: 'workerThreads' must be < 100.");

            this.workers = new Thread[workerThreads];

            // Create and start a separate thread for each worker
            for (int i = 0; i < workerThreads; i++)
            {
                this.workers[i] = new Thread(this.DoWork) { Name = string.Concat("BackgroundWorkerQueue", i) };
                this.workers[i].Start();
            }
        }

        /// <summary>
        /// Fires the <see cref="WorkFailed"/> events if any are registered.
        /// </summary>
        /// 
        /// <param name="e">
        /// The arguments for the event.
        /// </param>
        private void OnWorkFailed(WorkFailedProcessingEventArgs e)
        {
            // This is done to avoid a threading issue.
            var failedEvent = this.WorkFailed;

            if (failedEvent != null)
            {
                this.WorkFailed(this, e);
            }
        }

        /// <summary>
        /// Fires the <see cref="WorkSucceeded"/> events if any are registered.
        /// </summary>
        /// 
        /// <param name="e">
        /// The arguments for the event.
        /// </param>
        private void OnWorkSucceeded(WorkSucceededProcessingEventArgs e)
        {
            // This is done to avoid a threading issue.
            var succeedEventListeners = this.WorkSucceeded;

            if (succeedEventListeners != null)
            {
                this.WorkSucceeded(this, e);
            }
        }

        /// <summary>
        /// Stops the queue workers - no more work can be processed by this instance after a call to this method.
        /// </summary>
        public void Stop()
        {
            // We need to add as many poison pills to the queue as there are workers in order to ensure that
            // they all get stopped
            for (var i = 0; i < this.workers.Length; i++)
            {
                this.Enqueue(POISON_PILL);
            }
        }

        /// <summary>
        /// Enqueues the specified work.
        /// </summary>
        /// <param name="work">The work.</param>
        /// <returns><value>true</value> if the work was enqueued, else <value>false</value>.</returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="work"/> is null.
        /// </exception>
        public void Enqueue(IWork work)
        {
            if (work == null)
                throw new ArgumentException("Argument: 'work' must not be null.");

            lock (this.queueLocker)
            {
                this.queue.Enqueue(work);
                Monitor.Pulse(this.queueLocker);
            }
        }

        /// <summary>
        /// Gets the queue status.
        /// </summary>
        /// <returns>Queue status.</returns>
        public QueueStatus Status()
        {
            QueueStatus result;

            lock (this.queueLocker)
            {
                result = new QueueStatus(
                    new List<IWork>(this.queue), new List<IWork>(this.processing), new List<IWork>(this.failed));
            }
            
            return result;
        }

        /// <summary>
        /// Clears the current errors list.
        /// </summary>
        public void ClearErrors()
        {
            lock (this.queueLocker)
            {
                this.failed.Clear();
            }
        }

        /// <summary>
        /// Removes a specific failed items
        /// </summary>
        /// <param name="itemsToClear">A list of failed items</param>
        public void ClearErrors(IEnumerable<IWork> itemsToClear)
        {
            if (itemsToClear == null)
                throw new ArgumentException("Argument: 'itemsToClear' must not be null.");

            lock (this.queueLocker)
            {
                foreach (var itemToClear in itemsToClear)
                {
                    this.failed.Remove(itemToClear);
                }
            }
        }

        /// <summary>
        /// Re-adds a specific failed items
        /// </summary>
        /// <param name="failedItems">A list of failed items</param>
        public void ReAddFailed(IEnumerable<IWork> failedItems)
        {
            if (failedItems == null)
                throw new ArgumentException("Argument: 'failedItems' must not be null.");

            lock (this.queueLocker)
            {
                foreach (var failedItem in failedItems)
                {
                    if (this.failed.Remove(failedItem))
                    {
                        this.Enqueue(failedItem);
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Work performed by a worker thread.
        /// </summary>
        /// <remarks>
        /// A worker thread executes the below until it receives a <see cref="PoisonPill"/>.
        /// </remarks>
        private void DoWork()
        {
            // Keep consuming until receive poison pill 'null' task
            while (true)
            {
                IWork workToBeProcessed;

                lock (this.queueLocker)
                {
                    while (this.queue.Count == 0)
                    {
                        Monitor.Wait(this.queueLocker);
                    }

                    workToBeProcessed = this.queue.Dequeue();

                    if (ReferenceEquals(workToBeProcessed, POISON_PILL))
                    {
                        // Poison pill received - this signals our exit
                        break;
                    }

                    this.processing.Add(workToBeProcessed);
                }

                bool workSucceeded = false;
                string workFailedMsg = string.Empty;

                try
                {
                    workToBeProcessed.Process();

                    workSucceeded = true;
                }
                catch (ApplicationException ex)
                {
                    var message = string.Format("BackgroundWorkerQueue worker:{0}, Failed to process work:{1}",
                        Thread.CurrentThread.Name,
                        workToBeProcessed);
                    
                    workFailedMsg = message;
                }
                catch (Exception)
                {
                    var message = string.Format("BackgroundWorkerQueue worker:{0}, Unexpected exception while processing work:{1}",
                        Thread.CurrentThread.Name,
                        workToBeProcessed);

                    workFailedMsg = message;
                }

                lock (this.queueLocker)
                {
                    this.processing.Remove(workToBeProcessed);

                    if (workSucceeded)
                    {
                        var args = new WorkSucceededProcessingEventArgs(workToBeProcessed);
                        this.OnWorkSucceeded(args);
                    }
                    else
                    {
                        this.HandleFailedWork(workToBeProcessed, workFailedMsg);
                    }
                }
            }
        }

        private void HandleFailedWork(IWork workToBeProcessed, string message)
        {
            lock (this.queueLocker)
            {
                this.failed.Add(workToBeProcessed);

                var args = new WorkFailedProcessingEventArgs(workToBeProcessed, message);
                this.OnWorkFailed(args);
            }
        }

        /// <summary>
        /// Poison pill type used to kill the worker threads for the queue.
        /// </summary>
        private sealed class PoisonPill : IWork
        {
            /// <remarks>
            /// As this is a poison pill this process should never be executed.  An exception occurs if an attempt
            /// to process it occurs.
            /// </remarks>
            /// <exception cref="InvalidOperationException">
            /// If this method is called.
            /// </exception>
            public void Process()
            {
                throw new InvalidOperationException("A poison pill should not be processed.");
            }
        }
    }
}