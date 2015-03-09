using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdgisticsMotorsReport;

namespace AdgisticsMotors.Web.Services.Interfaces
{
    public interface IReportsService
    {
        IList<DealershipData> TopPerformingDealerships();
        IList<DealershipData> LowStockDealerships(int availableStock);
    }
}
