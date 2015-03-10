using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AdgisticsMotors.Web.Services.Interfaces;
using AdgisticsMotorsReport;
using AdgisticsMotorsReport.Utils.Threading;

namespace AdgisticsMotors.Web.Services
{
    public class ReportsService : IReportsService
    {
        IList<IWork> workList;
        BackgroundWorkerQueue backgroundWorkerQueue = new BackgroundWorkerQueue(1);
        ConcurrentBag<DealershipData> concurrentBag = new ConcurrentBag<DealershipData>();

        private IDealershipsLoaderService _dealershipsLoaderService;

        public ReportsService(IDealershipsLoaderService dealershipsLoaderService)
        {
            _dealershipsLoaderService = dealershipsLoaderService;

            backgroundWorkerQueue.WorkSucceeded += backgroundWorkerQueue_WorkSucceeded;
            backgroundWorkerQueue.WorkFailed += backgroundWorkerQueue_WorkFailed;
        }

        public IList<DealershipData> TopPerformingDealerships()
        {
            var dealershipsList = _dealershipsLoaderService.LoadServices();

            try
            {
                foreach (var dealershipData in dealershipsList)
                {
                    backgroundWorkerQueue.Enqueue((new TopPerformingDealershipsWork(dealershipData.Id, dealershipData.Uri)));
                }
            }
            catch (ArgumentException exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.ToString()); //This should be changed to inform user about exception
                throw;
            }

            var status = backgroundWorkerQueue.Status();
            while (status.Backlog.Any() || status.Processing.Any())
            {
                status = backgroundWorkerQueue.Status();
            }
            if(status.Failed.Any())
            {
                backgroundWorkerQueue.ReAddFailed(status.Failed);
                status = backgroundWorkerQueue.Status();
            }
            backgroundWorkerQueue.Dispose();

            return concurrentBag.ToList();
        }

        private void backgroundWorkerQueue_WorkFailed(object sender, WorkFailedProcessingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message.ToString());
        }

        private void backgroundWorkerQueue_WorkSucceeded(object sender, WorkSucceededProcessingEventArgs e)
        {
            var result = (e.Work as TopPerformingDealershipsWork).DealershipData;
            
            concurrentBag.Add(result);
        }

        public IList<DealershipData> LowStockDealerships(int availableStock)
        {
            throw new NotImplementedException();
        }
    }

    public class TopPerformingDealershipsWork : IWork
    {
        private readonly string _id;
        private readonly Uri _uri;
        private readonly DealershipService _dealershipService;
        public DealershipData DealershipData { get; private set; }

        public TopPerformingDealershipsWork(string id, Uri uri)
        {
            this._id = id;
            this._uri = uri;
            this._dealershipService = new DealershipService();
        }

        public void Process()
        {
            DealershipData = _dealershipService.GetDealershipData(_id, _uri);
        }
    }
}
