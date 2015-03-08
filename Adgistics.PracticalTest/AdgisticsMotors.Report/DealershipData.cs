namespace AdgisticsMotorsReport
{
    using System;

    /// <summary>
    /// Data for an individual dealership of Adgistics Motors
    /// </summary>
    public sealed class DealershipData
    {
        /// <summary>
        /// Gets the identifier of the dealership.
        /// </summary>
        public string DealershipIdentifier { get; set; }

        /// <summary>
        /// Gets the total sales for the dealership.
        /// </summary>
        public Decimal TotalSales { get; set; }

        /// <summary>
        /// Gets the remaining stock count for the dealership. i.e. how many cars they have in stock still.
        /// </summary>
        public int AvailableStock { get; set; }
    }
}