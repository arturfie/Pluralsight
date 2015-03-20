using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AdgisticsMotors.Web.Infrastructure.Works;
using AdgisticsMotors.Web.Models;
using AdgisticsMotors.Web.Services.Interfaces;
using AdgisticsMotorsReport;
using AdgisticsMotorsReport.Utils.Threading;

namespace AdgisticsMotors.Web.Services
{
    public class ReportsService : IReportsService
    {
        private const int LowStockLevel = 10;
        private const int TopPerformingDealershipsNumber = 100;
        IList<DealershipInfo> _dealershipsList;
        private readonly BackgroundWorkerQueue _backgroundWorkerQueue = new BackgroundWorkerQueue(99);
        private readonly ConcurrentQueue<DealershipData> _concurrentBag = new ConcurrentQueue<DealershipData>();

        private IDealershipsLoaderService _dealershipsLoaderService;
        
        public ReportsService(IDealershipsLoaderService dealershipsLoaderService)
        {
            _dealershipsLoaderService = dealershipsLoaderService;
            
            _backgroundWorkerQueue.WorkFailed += backgroundWorkerQueue_WorkFailed;
        }

        public IList<DealershipData> TopPerformingDealerships()
        {
            _dealershipsList = _dealershipsLoaderService.LoadServices();

            try
            {
                _backgroundWorkerQueue.WorkSucceeded += backgroundWorkerQueue_WorkSucceeded;
                foreach (var dealershipData in _dealershipsList)
                {
                    _backgroundWorkerQueue.Enqueue((new GetDealershipDataWork(dealershipData.Id, dealershipData.Uri)));
                }
                ProcessDealershipsWork();
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
            var topDealersList = _concurrentBag.OrderByDescending(x => x.TotalSales).Take(TopPerformingDealershipsNumber).ToList();

            return topDealersList;
        }

        private void ProcessDealershipsWork()
        {
            var status = _backgroundWorkerQueue.Status();
            while (status.Backlog.Any() || status.Processing.Any())
            {
                status = _backgroundWorkerQueue.Status();
            }
            if (status.Failed.Any())
            {
                _backgroundWorkerQueue.ReAddFailed(status.Failed);
                ProcessDealershipsWork();
            }
        }

        private void backgroundWorkerQueue_WorkSucceeded(object sender, WorkSucceededProcessingEventArgs e)
        {
            var result = (e.Work as GetDealershipDataWork).DealershipData;
            _concurrentBag.Enqueue(result);
        }

        public IList<DealershipData> LowStockDealerships()
        {
            _dealershipsList = _dealershipsLoaderService.LoadServices();
            try
            {
                _backgroundWorkerQueue.WorkSucceeded += backgroundWorkerQueue_WorkSucceeded_LowStock;
                foreach (var dealershipData in _dealershipsList)
                {
                    _backgroundWorkerQueue.Enqueue((new GetDealershipDataWork(dealershipData.Id, dealershipData.Uri)));
                }
                ProcessDealershipsWork();
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

        private void backgroundWorkerQueue_WorkSucceeded_LowStock(object sender, WorkSucceededProcessingEventArgs e)
        {
            var result = (e.Work as GetDealershipDataWork).DealershipData;
            if (result.AvailableStock < LowStockLevel)
            {
                _concurrentBag.Enqueue(result);
            }
        }

        private void backgroundWorkerQueue_WorkFailed(object sender, WorkFailedProcessingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            //_logger.Log(e.Message);
        }
    }
}
