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

    /// <summary>
    /// A class that represents the arguments for an event
    /// that indicates processing has failed on a <see cref="IWork"/> element.
    /// </summary>
    public sealed class WorkFailedProcessingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFailedProcessingEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="work">
        /// The item of <see cref="IWork"/> that has failed.
        /// </param>
        /// 
        /// <param name="message">
        /// A message that indicates what has failed.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="work"/> is null.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="message"/> is null, blank or whitespace.
        /// </exception>
        public WorkFailedProcessingEventArgs(IWork work, string message)
        {
            if (work == null)
                throw new ArgumentException("Argument: 'work' may not be null.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Argument: 'message' may not be null, blank or whitespace.");

            this.Message = message;
            this.Work = work;
        }

        /// <summary>
        /// Gets the failed work item.
        /// </summary>
        public IWork Work
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message associated with the failure.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }
    }
}