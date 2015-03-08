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
    /// Represents work that can be processed by the <see cref="ApplicationException"/>.
    /// </summary>
    public interface IWork
    {
        /// <summary>
        /// Processes this instance.
        /// </summary>
        /// <remarks>
        /// All the work required for this instance is performed in this method.
        /// </remarks>
        /// <exception cref="ApplicationException">
        /// If processing fails for any reason.
        /// </exception>
        void Process();
    }
}