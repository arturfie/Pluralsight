using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using AdgisticsMotors.Web.Models;
using AdgisticsMotors.Web.Services.Interfaces;
using AdgisticsMotorsReport;
using AdgisticsMotorsReport.Utils.Threading;

namespace AdgisticsMotors.Web.Services
{
    public class ReportsService : IReportsService
    {
        private readonly BackgroundWorkerQueue _backgroundWorkerQueue = new BackgroundWorkerQueue(99);
        private readonly ConcurrentQueue<DealershipData> _concurrentBag = new ConcurrentQueue<DealershipData>();

        private IDealershipsLoaderService _dealershipsLoaderService;

        public ReportsService(IDealershipsLoaderService dealershipsLoaderService)
        {
            _dealershipsLoaderService = dealershipsLoaderService;

            _backgroundWorkerQueue.WorkSucceeded += backgroundWorkerQueue_WorkSucceeded;
            _backgroundWorkerQueue.WorkFailed += backgroundWorkerQueue_WorkFailed;
        }

        public IList<DealershipData> TopPerformingDealerships()
        {
            var dealershipsQueue = new Queue<DealershipInfo>(_dealershipsLoaderService.LoadServices());

            try
            {
                foreach (var dealershipData in dealershipsQueue)
                {
                    _backgroundWorkerQueue.Enqueue((new TopPerformingDealershipsWork(dealershipData.Id, dealershipData.Uri)));
                }
                ProcessTopDealersWork();
            }
            catch (ArgumentException exception)
            {
                //_logger.Log(exception.Message);
                throw;
            }
            finally
            {
                _backgroundWorkerQueue.Dispose();
            }

            return _concurrentBag.ToList();
        }



        private void ProcessTopDealersWork()
        {
            var status = _backgroundWorkerQueue.Status();
            while (status.Backlog.Any() || status.Processing.Any())
            {
                status = _backgroundWorkerQueue.Status();
            }
            if (status.Failed.Any())
            {
                _backgroundWorkerQueue.ReAddFailed(status.Failed);
                ProcessTopDealersWork();
            }
        }

        private void backgroundWorkerQueue_WorkFailed(object sender, WorkFailedProcessingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            //_logger.Log(e.Message);
        }

        private void backgroundWorkerQueue_WorkSucceeded(object sender, WorkSucceededProcessingEventArgs e)
        {
            var result = (e.Work as TopPerformingDealershipsWork).DealershipData;
            _concurrentBag.Enqueue(result);
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
