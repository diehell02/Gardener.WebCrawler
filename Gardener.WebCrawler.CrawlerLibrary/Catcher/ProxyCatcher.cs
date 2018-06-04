using System;
using System.Collections.Generic;
using Gardener.WebCrawler.Contracts.Data;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.CrawlerLibrary.Factory;
using Gardener.WebCrawler.CrawlerLibrary.ProxyHandle;

namespace Gardener.WebCrawler.CrawlerLibrary.Catcher
{
    class ProxyCatcher : Catcher
    {
        List<ProxyWebApi> proxyWebApis;
        ProxyFactory proxyFactory;

        public ProxyCatcher(IHelper<ProxyWebApi> proxyWebApiHelper, IHelper<Proxy> proxyHelper):base(proxyHelper)
        {
            proxyWebApis = proxyWebApiHelper.Get();
            proxyFactory = new ProxyFactory();
        }

        protected override IEnumerable<ICatchItem> GetCatchList()
        {
            return proxyWebApis;
        }

        protected override void WebResponseHandle(string response, ICatchItem catchItem)
        {
            try
            {
                string className = (string)catchItem.Extend;

                IProxyHandle handle = proxyFactory.GetProxyHandle(className);

                if(handle == null)
                {
                    return;
                }

                List<Proxy> proxyList = handle.DoHandle(response);

                if(proxyList == null)
                {
                    return;
                }

                proxyList.ForEach(proxy =>
                {
                    httpUtil.VerifyProxy(proxy.Address, isSuccess =>
                    {
                        if(isSuccess)
                        {
                            proxyHelper.Add(proxy);
                            logger.InfoFormat("添加代理:{0}", proxy.Address.AbsoluteUri);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error ProxyHandle:{0}", ex);
            }         
        }
    }
}
