using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Gardener.WebCrawler.Contracts.Entity
{
    public interface ICatchItem
    {
        Uri Uri { get; }

        Uri Proxy { get; set; }

        object Extend { get; }

        string Cookie { get; }
    }
}
