using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdgisticsMotorsReport.Interfaces
{
    public interface IDealershipService
    {
        DealershipData GetDealershipData(string dealershipIdentifier, Uri dealershipEndpoint);
    }
}
