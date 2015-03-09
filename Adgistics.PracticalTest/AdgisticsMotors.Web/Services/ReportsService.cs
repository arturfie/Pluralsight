using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AdgisticsMotors.Web.Services.Interfaces;
using AdgisticsMotorsReport;
using AdgisticsMotorsReport.Utils.Threading;

namespace AdgisticsMotors.Web.Services
{
    public class ReportsService : IReportsService
    {
        IList<IWork> workList;
        BackgroundWorkerQueue backgroundWorkerQueue = new BackgroundWorkerQueue(99);
        ConcurrentBag<DealershipData> concurrentBag = new ConcurrentBag<DealershipData>();
        private bool semaphore = true;
        IList<IWork> failedList = new List<IWork>();

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
                System.Diagnostics.Debug.WriteLine(exception.ToString());
                throw;
            }

            var status = backgroundWorkerQueue.Status();
            while (status.Backlog.Count() > 0)
            {
                if (failedList.Count() > 0)
                {
                    backgroundWorkerQueue.ReAddFailed(failedList);
                    failedList.Clear();
                }
            }
            var list = concurrentBag.ToList();
            return list;
        }

        private void backgroundWorkerQueue_WorkFailed(object sender, WorkFailedProcessingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message.ToString());
            failedList.Add(e.Work);
        }

        private void backgroundWorkerQueue_WorkSucceeded(object sender, WorkSucceededProcessingEventArgs e)
        {
            var result = (e.Work as TopPerformingDealershipsWork).DealershipData;
            
            concurrentBag.Add(result);
            semaphore = false;
        }

        public IList<DealershipData> LowStockDealerships(int availableStock)
        {
            throw new NotImplementedException();
        }
    }

    public class TopPerformingDealershipsWork : IWork
    {
        private readonly DealershipService _dealershipService;
        private readonly string _id;
        private readonly Uri _uri;
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
