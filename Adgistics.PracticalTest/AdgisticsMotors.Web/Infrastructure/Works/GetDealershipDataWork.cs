using System;
using AdgisticsMotorsReport;
using AdgisticsMotorsReport.Utils.Threading;

namespace AdgisticsMotors.Web.Infrastructure.Works
{
    public class GetDealershipDataWork : IWork
    {
        private readonly string _id;
        private readonly Uri _uri;
        private readonly DealershipService _dealershipService;
        public DealershipData DealershipData { get; private set; }

        public GetDealershipDataWork(string id, Uri uri)
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
