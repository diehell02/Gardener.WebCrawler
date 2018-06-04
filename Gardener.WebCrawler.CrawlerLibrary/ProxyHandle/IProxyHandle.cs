using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.Contracts.Entity;

namespace Gardener.WebCrawler.CrawlerLibrary.ProxyHandle
{
    interface IProxyHandle
    {
        List<Proxy> DoHandle(string response);
    }
}
