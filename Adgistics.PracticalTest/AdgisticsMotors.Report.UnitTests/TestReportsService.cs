using System;
using System.Collections.Generic;
using AdgisticsMotors.Web.Models;
using AdgisticsMotors.Web.Services;
using AdgisticsMotors.Web.Services.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace AdgisticsMotorsReport.Tests
{
    [TestFixture]
    public class TestReportsService
    {
        IList<DealershipInfo> listOfDealerships = new List<DealershipInfo>()
            {
                new DealershipInfo("1", new Uri("http://www.test.com")),
                new DealershipInfo("2", new Uri("http://www.test2.com"))
            };

        [Test]
        public void When_Executing_TopPerformingDealerships()
        {
            //ARRANGE
            var listOfDealerships = new List<DealershipInfo>()
            {
                new DealershipInfo("1", new Uri("http://www.test.com")),
                new DealershipInfo("2", new Uri("http://www.test2.com"))
            };
            var dealershipsLoaderServiceMock = MockRepository.GenerateStub<IDealershipsLoaderService>();
            dealershipsLoaderServiceMock.Stub(x => x.LoadServices()).Return(listOfDealerships);
            var reportsService = new ReportsService(dealershipsLoaderServiceMock);

            //ACT
            var returnedList = reportsService.TopPerformingDealerships();

            //ASSERT
            Assert.IsNotEmpty(returnedList);
            Assert.IsInstanceOf<IList<DealershipData>>(returnedList);
        }

        [Test]
        public void When_Executing_TopPerformingDealerships_And_List_Of_Dealerships_Is_Empty()
        {
            //ARRANGE

            var dealershipsLoaderServiceMock = MockRepository.GenerateStub<IDealershipsLoaderService>();
            dealershipsLoaderServiceMock.Stub(x => x.LoadServices()).Return(new List<DealershipInfo>());
            var reportsService = new ReportsService(dealershipsLoaderServiceMock);

            //ACT
            var returnedList = reportsService.TopPerformingDealerships();

            //ASSERT
            Assert.IsEmpty(returnedList);
        }

        [Test]
        public void When_Executing_LowStockDealerships()
        {
            //ARRANGE
            var dealershipsLoaderServiceMock = MockRepository.GenerateStub<IDealershipsLoaderService>();
            dealershipsLoaderServiceMock.Stub(x => x.LoadServices()).Return(listOfDealerships);
            var reportsService = new ReportsService(dealershipsLoaderServiceMock);

            //ACT
            var returnedList = reportsService.LowStockDealerships();

            //ASSERT
            Assert.IsNotEmpty(returnedList);
            Assert.IsInstanceOf<IList<DealershipData>>(returnedList);
        }
    }
}
