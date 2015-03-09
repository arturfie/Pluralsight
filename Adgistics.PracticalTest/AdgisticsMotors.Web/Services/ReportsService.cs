using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdgisticsMotors.Web.Services.Interfaces;
using AdgisticsMotorsReport;
using AdgisticsMotorsReport.Interfaces;
using AdgisticsMotorsReport.Utils.Threading;

namespace AdgisticsMotors.Web.Services
{
    public class ReportsService : IReportsService
    {
        public IList<DealershipData> TopPerformingDealerships()
        {
            IList<DealershipData> dealershipList = new List<DealershipData>();
            BackgroundWorkerQueue backgroundWorkerQueue = new BackgroundWorkerQueue(99);
            IWork work = new TopPerformingDealershipsWork(new DealershipService());
            
            backgroundWorkerQueue.Enqueue(work);

            return dealershipList;
        }

        public IList<DealershipData> LowStockDealerships(int availableStock)
        {
            throw new NotImplementedException();
        }
    }

    public class TopPerformingDealershipsWork : IWork
    {
        private readonly IDealershipService _dealershipService;

        public TopPerformingDealershipsWork(IDealershipService dealershipService)
        {
            _dealershipService = dealershipService;
        }

        public void Process()
        {
            _dealershipService.GetDealershipData("", new Uri(""));
        }
    }
}
