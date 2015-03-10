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

            try
            {
                foreach (string line in File.ReadLines(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath + "\\DealershipsList.txt"))
                {
                    if (dealershipList.Count() > 1000)
                    {
                        break;
                    }
                    var lineSplited = line.Split(',');

                    dealershipList.Add(new DealershipInfo(lineSplited[0], new Uri(lineSplited[1])));
                }
            }
            //I would inject logger service to log exceptions into database
            //catch (UnauthorizedAccessException UAEx)
            //{
            //    _logger.Log(UAEx.Message);
            //    throw 
            //}
            //catch (PathTooLongException PathEx)
            //{
            //    _logger.Log(PathEx.Message);
            //    throw 
            //}
            //catch (PathTooLongException PathEx)
            //{
            //    _logger.Log(PathEx.Message);
            //    throw 
            //}
            //catch (UriFormatException UriEx)
            //{
            //    _logger.Log(.Message);
            //    throw 
            //}
            catch (Exception exception)
            {
            //  _logger.Log(exception.Message);
                throw;
            }
            return dealershipList;
        }
    }
}
