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
    public sealed class WorkSucceededProcessingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkSucceededProcessingEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="work">
        /// The item of <see cref="IWork"/> that has been successfully processed.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="work"/> is null.
        /// </exception>
        public WorkSucceededProcessingEventArgs(IWork work)
        {
            if (work == null)
                throw new ArgumentException("Argument: 'work' may not be null.");

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
    }
}