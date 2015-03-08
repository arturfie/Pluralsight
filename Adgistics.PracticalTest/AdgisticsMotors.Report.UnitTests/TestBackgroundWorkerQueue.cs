// Copyright (c) Adgistics Limited and others. All rights reserved. 
// The contents of this file are subject to the terms of the 
// Adgistics Development and Distribution License (the "License"). 
// You may not use this file except in compliance with the License.
// 
// http://www.adgistics.com/license.html
// 
// See the License for the specific language governing permissions
// and limitations under the License.
namespace AdgisticsMotorsReport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using AdgisticsMotorsReport.Utils.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class TestBackgroundWorkerQueue
    {
        /// <summary>
        /// Stores the successfully processed work items - aids test assertions.
        /// </summary>
        private static readonly HashSet<StandardWork> PROCESSED_WORK = new HashSet<StandardWork>();

        [SetUp]
        public void SetUp()
        {
            PROCESSED_WORK.Clear();
        }

        /// <remarks>
        /// As we have 10 worker threads, with only 10 items added to queue (each of which has a maximum of 1 second
        /// processing time), we should be able to execute all of them within 1.5 secs (or less even).
        /// </remarks>
        [Test, Timeout(1500)]
        public void AllItemsAreProcessed()
        {
            // Arrange
            var workerThreads = 10;
            var worker = new BackgroundWorkerQueue(workerThreads);

            var work = new List<StandardWork>();

            for (var i = 0; i < 10; i++)
            {
                work.Add(new StandardWork());
            }

            // Act
            foreach (var w in work)
            {
                worker.Enqueue(w);
            }

            // Assert
            while (true)
            {
                if (PROCESSED_WORK.SetEquals(work))
                {
                    Assert.Pass("All work items were processed.");
                }
            }
        }

        [Test]
        public void NoItemsAreProcessedAfterStop()
        {
            // Arrange
            var workerThreads = 10;
            var worker = new BackgroundWorkerQueue(workerThreads);

            var work = new List<StandardWork>();

            for (var i = 0; i < 10; i++)
            {
                work.Add(new StandardWork());
            }

            // Act
            worker.Stop();

            foreach (var w in work)
            {
                worker.Enqueue(w);
            }

            // wait for a bit
            Thread.Sleep(500);

            // Assert
            Assert.IsEmpty(PROCESSED_WORK);
        }

        [Test]
        public void QueueIsNotBrokenByExceptions()
        {
            // Arrange
            var workerThreads = 2;
            var worker = new BackgroundWorkerQueue(workerThreads);

            var exceptionThrowingWork = new ExceptionThrowingWork();
            var work = new StandardWork();

            // Act
            worker.Enqueue(exceptionThrowingWork);
            worker.Enqueue(work);

            // wait for a bit
            Thread.Sleep(1000);

            // Assert
            CollectionAssert.AreEquivalent(new List<StandardWork>() { work }, PROCESSED_WORK);
        }

        [Test]
        public void Status()
        {
            // Arrange
            var workerThreads = 2;

            var worker = new BackgroundWorkerQueue(workerThreads);

            var exceptionThrowingWork1 = new ExceptionThrowingWork();
            var exceptionThrowingWork2 = new ExceptionThrowingWork();
            var longRunningWork1 = new LongRunningWork();
            var longRunningWork2 = new LongRunningWork();
            var work1 = new StandardWork();
            var work2 = new StandardWork();

            // Act
            worker.Enqueue(exceptionThrowingWork1);
            worker.Enqueue(exceptionThrowingWork2);
            worker.Enqueue(longRunningWork1);
            worker.Enqueue(longRunningWork2);
            worker.Enqueue(work1);
            worker.Enqueue(work2);

            // wait for a bit
            Thread.Sleep(1000);

            var actual = worker.Status();

            // Assert

            //backlog
            CollectionAssert.AreEquivalent(new List<IWork>() { work1, work2 }, actual.Backlog);

            //failed
            CollectionAssert.AreEquivalent(
                new List<IWork>() { exceptionThrowingWork1, exceptionThrowingWork2 }, actual.Failed);

            //processing
            CollectionAssert.AreEquivalent(new List<IWork>() { longRunningWork1, longRunningWork2 }, actual.Processing);
        }

        [Test]
        public void ClearErrors()
        {
            // ARRANGE
            var workerThreads = 1;

            var worker = new BackgroundWorkerQueue(workerThreads);

            var exceptionThrowingWork = new ExceptionThrowingWork();

            worker.Enqueue(exceptionThrowingWork);

            // wait for a bit
            Thread.Sleep(500);

            var status = worker.Status();

            // PRECONDITIONS ASSERT
            CollectionAssert.IsNotEmpty(status.Failed);

            // ACT
            worker.ClearErrors();

            // ASSERT
            var actual = worker.Status().Failed;

            CollectionAssert.IsEmpty(actual);
        }

        [Test]
        public void ClearSpecificErrors()
        {
            // ARRANGE
            var workerThreads = 1;

            var worker = new BackgroundWorkerQueue(workerThreads);

            var exceptionThrowingWork = new ExceptionThrowingWork();

            var longRunningWorkProcessing = new LongRunningWork();
            var longRunningWorkInBacklog = new LongRunningWork();

            worker.Enqueue(exceptionThrowingWork);
            worker.Enqueue(longRunningWorkProcessing);
            worker.Enqueue(longRunningWorkInBacklog);

            // wait for a bit
            Thread.Sleep(500);

            var status = worker.Status();

            // PRECONDITIONS ASSERT
            CollectionAssert.IsNotEmpty(status.Failed);

            // ACT
            worker.ClearErrors(new List<IWork>() { exceptionThrowingWork });

            // ASSERT
            var actual = worker.Status();

            CollectionAssert.IsEmpty(actual.Failed);
            CollectionAssert.Contains(actual.Processing, longRunningWorkProcessing);
            CollectionAssert.Contains(actual.Backlog, longRunningWorkInBacklog);
        }

        [Test]
        public void ReaddSpecificErrors()
        {
            // ARRANGE
            var workerThreads = 1;

            var worker = new BackgroundWorkerQueue(workerThreads);

            var exceptionThrowingWork = new ExceptionThrowingWork();

            var longRunningWorkProcessing = new LongRunningWork();
            var longRunningWorkInBacklog = new LongRunningWork();

            worker.Enqueue(exceptionThrowingWork);
            worker.Enqueue(longRunningWorkProcessing);
            worker.Enqueue(longRunningWorkInBacklog);

            // wait for a bit
            Thread.Sleep(500);

            var status = worker.Status();

            // PRECONDITIONS ASSERT
            CollectionAssert.IsNotEmpty(status.Failed);

            // ACT
            worker.ReAddFailed(new List<IWork>() { exceptionThrowingWork });

            // ASSERT
            var actual = worker.Status();

            CollectionAssert.IsEmpty(actual.Failed);
            CollectionAssert.Contains(actual.Processing, longRunningWorkProcessing);
            CollectionAssert.Contains(actual.Backlog, longRunningWorkInBacklog);
            CollectionAssert.Contains(actual.Backlog, exceptionThrowingWork);
        }

        [Test, Timeout(3000)]
        public void WhenWorkSucceeded()
        {
            // ARRANGE
            var workerThreads = 1;
            var worker = new BackgroundWorkerQueue(workerThreads);

            var onSucceedTestWork = new OnSucceedTestWork(worker);

            // ACT
            worker.Enqueue(onSucceedTestWork);

            // ASSERT
            while (false == onSucceedTestWork.Called)
            {
                // wait for called
            }

            Assert.Pass("The OnWorkSucceed event fired");
        }

        [Test, Timeout(3000)]
        public void WhenWorkFailed()
        {
            // ARRANGE
            var workerThreads = 1;
            var worker = new BackgroundWorkerQueue(workerThreads);

            var onFailedTestWork = new OnFailedTestWork(worker);

            // ACT
            worker.Enqueue(onFailedTestWork);

            // ASSERT
            while (false == onFailedTestWork.Called)
            {
                // wait for called
            }

            Assert.Pass("The OnWorkFailed event fired");
        }

        /// <summary>
        /// Dummy test work used to test the OnFailed event handling
        /// </summary>
        public class OnFailedTestWork : IWork
        {
            private readonly BackgroundWorkerQueue workerQueue;

            public bool Called = false;

            public OnFailedTestWork(BackgroundWorkerQueue workerQueue)
            {
                this.workerQueue = workerQueue;
            }

            #region Implementation of IWork

            /// <summary>
            /// Processes this instance.
            /// </summary>
            /// <remarks>
            /// All the work required for this instance is performed in this method.
            /// </remarks>
            /// <exception cref="ApplicationException">
            /// If processing fails for any reason.
            /// </exception>
            public void Process()
            {
                this.workerQueue.WorkFailed += this.OnWorkFailed;

                throw new Exception("Error thrown to test on failed handler.");
            }

            #endregion

            void OnWorkFailed(object sender, WorkFailedProcessingEventArgs e)
            {
                this.Called = true;
            }
        }

        /// <summary>
        /// Dummy test work used to test the OnSuceeded event handling
        /// </summary>
        public class OnSucceedTestWork : IWork
        {
            private readonly BackgroundWorkerQueue workerQueue;

            public bool Called = false;

            public OnSucceedTestWork(BackgroundWorkerQueue workerQueue)
            {
                this.workerQueue = workerQueue;
            }

            #region Implementation of IWork

            /// <summary>
            /// Processes this instance.
            /// </summary>
            /// <remarks>
            /// All the work required for this instance is performed in this method.
            /// </remarks>
            /// <exception cref="ApplicationException">
            /// If processing fails for any reason.
            /// </exception>
            public void Process()
            {
                this.workerQueue.WorkSucceeded += this.OnWorkSucceed;
            }

            #endregion

            void OnWorkSucceed(object sender, WorkSucceededProcessingEventArgs e)
            {
                this.Called = true;
            }
        }

        /// <summary>
        /// Dummy work implementation to aid in testing.  This is a standard worker.  It will emulate work
        /// that takes between 0.5 and 1 seconds.  All processed items will get added to <see cref="PROCESSED_WORK"/>.
        /// </summary>
        private sealed class StandardWork : DummyWorkBase
        {
            private static readonly Random RANDOM = new Random();

            public override void Process()
            {
                // fake some work being done
                var workTime = RANDOM.Next(500, 1000); // between half a second and a full second

                Thread.Sleep(workTime);

                // store the fact that work is done
                PROCESSED_WORK.Add(this);
            }

            public override string ToString()
            {
                return String.Format("StandardWork:{{ Id:{0} }}", this.Id);
            }
        }

        /// <summary>
        /// Dummy work implementation to aid in testing.  This is a long running work item.  It runs for 2 minutes.
        /// This provides us with a better capability with which to test the status method.
        /// </summary>
        private sealed class LongRunningWork : DummyWorkBase
        {
            private const int TWO_MINUTES = 2 /*mins*/* 60 /*secs*/* 1000;

            public override void Process()
            {
                Thread.Sleep(TWO_MINUTES);
            }

            public override string ToString()
            {
                return String.Format("LongRunningWork:{{ Id:{0} }}", this.Id);
            }
        }

        /// <summary>
        /// Dummy work implementation to aid in testing.  This is an exception throwing work item, so that we can
        /// emulate work items that fail.
        /// </summary>
        private sealed class ExceptionThrowingWork : DummyWorkBase
        {
            public override void Process()
            {
                throw new Exception("This work process is broken.");
            }

            public override string ToString()
            {
                return String.Format("ExceptionThrowingWork:{{ Id:{0} }}", this.Id);
            }
        }

        private abstract class DummyWorkBase : IWork
        {
            private static int uniqueIdCounter = 0;

            public DummyWorkBase()
            {
                this.Id = uniqueIdCounter++;
            }

            protected int Id { get; set; }

            public abstract void Process();

            private bool Equals(DummyWorkBase other)
            {
                return this.Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                return obj is DummyWorkBase && this.Equals((DummyWorkBase)obj);
            }

            public override int GetHashCode()
            {
                return this.Id;
            }
        }
    }
}