// Copyright (c) Adgistics Limited and others. All rights reserved. 
// The contents of this file are subject to the terms of the 
// Adgistics Development and Distribution License (the "License"). 
// You may not use this file except in compliance with the License.
// 
// http://www.adgistics.com/license.html
// 
// See the License for the specific language governing permissions
// and limitations under the License.


namespace AdgisticsMotorsReport
{
    using System;
    using System.Threading;
    using System.Web;
    using Interfaces;

    /// <summary>
    /// Delearship Service class used to make "web service" requests to fetch the data for dealerships.
    /// </summary>
    public sealed class DealershipService : IDealershipService
    {
        private static readonly Random random = new Random();

        /// <summary>
        ///   Gets the dealership data for the given dealership identifier and endpoint.
        /// </summary>
        /// 
        /// <param name="dealershipIdentifier">The dealership identifier.</param>
        /// <param name="dealershipEndpoint">The dealership endpoint address.</param>
        /// 
        /// <returns>The <see cref="DealershipData"/> for the given dealer.</returns>
        /// 
        /// <exception cref="System.ArgumentException">
        ///   <para>Argument: 'dealershipIdentifier' must not be null, whitespace only, or empty.</para>
        ///   or
        ///   <para>Argument: 'dealershipEndpoint' must not be null</para>
        /// </exception>
        /// 
        /// <exception cref="System.Web.HttpException">
        ///   500 - If a failure occurs while attempting to communicate with the dealership end point.
        /// </exception>
        public DealershipData GetDealershipData(string dealershipIdentifier, Uri dealershipEndpoint)
        {
            if (string.IsNullOrWhiteSpace(dealershipIdentifier))
                throw new ArgumentException(
                    "Argument: 'dealershipIdentifier' must not be null, whitespace only, or empty.");
            if (dealershipEndpoint == null)
                throw new ArgumentException(
                    "Argument: 'dealershipEndpoint' must not be null");

            // we will randomly wait between 100 and 2000 milliseconds to simulate network contention
            Thread.Sleep(random.Next(100, 2000));

            // we will randomly throw an HttpException to simulate a network failure
            var randomResultIndicator = random.Next(1, 5);
            if (randomResultIndicator == 1)
            {
                throw new HttpException(500, string.Format("Failed to connect to service:{0}", dealershipEndpoint));
            }

            // we generate a random set of data for the dealership to return as the result
            var result = new DealershipData
                {
                    DealershipIdentifier = dealershipIdentifier, 
                    TotalSales = random.Next(125000, 15000000),
                    AvailableStock = random.Next(0, 50)
                };

            return result;
        }
    }
}