using System;

namespace AdgisticsMotors.Web.Models
{
    public sealed class DealershipInfo
    {
        public string Id { get; private set; }
        public Uri Uri { get; private set; }

        public DealershipInfo(string id, Uri uri)
        {
            Id = id;
            Uri = uri;
        }
    }
}
