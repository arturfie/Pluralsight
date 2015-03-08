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
    using System.Linq;

    /// <summary>
    /// Represents the current status of a <see cref="BackgroundWorkerQueue"/>.
    /// </summary>
    public sealed class QueueStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueStatus"/> class.
        /// </summary>
        /// <param name="outstanding">The outstanding.</param>
        /// <param name="processing">The processing.</param>
        /// <param name="failed">The failed.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="outstanding"/> is null.
        /// If <paramref name="processing"/> is null.
        /// If <paramref name="failed"/> is null.
        /// </exception>
        public QueueStatus(IEnumerable<IWork> outstanding, IEnumerable<IWork> processing, IEnumerable<IWork> failed)
        {
            if (outstanding == null) 
                throw new ArgumentException("Argument: 'outstanding' must not be null.");
            if (processing == null) 
                throw new ArgumentException("Argument: 'processing' must not be null.");
            if (failed == null) 
                throw new ArgumentException("Argument: 'failed' must not be null.");

            this.Backlog = outstanding;
            this.Processing = processing;
            this.Failed = failed;
        }

        /// <summary>
        /// Gets the number of oustanding items. i.e. the queue backlog
        /// </summary>
        public IEnumerable<IWork> Backlog { get; private set; }

        /// <summary>
        /// Gets the number of items currently being processed.
        /// </summary>
        public IEnumerable<IWork> Processing { get; private set; }

        /// <summary>
        /// Gets the failed items.
        /// </summary>
        public IEnumerable<IWork> Failed { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "QueueStatus:{{ Backlog.Count:{0}, Processing.Count:{1}, Failed.Count:{2} }}",
                this.Backlog.Count(),
                this.Processing.Count(),
                this.Failed.Count());
        }
    }
}