using System.Collections.Generic;
using AdgisticsMotors.Web.Models;

namespace AdgisticsMotors.Web.Services.Interfaces
{
    public interface IDealershipsLoaderService
    {
        IList<DealershipInfo> LoadServices();
    }
}
