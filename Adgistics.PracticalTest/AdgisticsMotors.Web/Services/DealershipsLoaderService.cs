using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdgisticsMotors.Web.Models;
using AdgisticsMotors.Web.Services.Interfaces;

namespace AdgisticsMotors.Web.Services
{
    public class DealershipsLoaderService : IDealershipsLoaderService
    {
        public Exception Exception { get; set; }
        public IList<DealershipInfo> LoadServices()
        {
            IList<DealershipInfo> dealershipList = new List<DealershipInfo>();

            foreach (string line in File.ReadLines(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath + "\\DealershipsList.txt"))
            {
                if (dealershipList.Count() > 100)
                {
                    break;
                }
                var lineSplited = line.Split(',');

                dealershipList.Add(new DealershipInfo(lineSplited[0], new Uri(lineSplited[1])));
            }

            return dealershipList;
        }
    }
}
