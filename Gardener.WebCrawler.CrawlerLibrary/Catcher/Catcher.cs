using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.Contracts.Data;

namespace Gardener.WebCrawler.CrawlerLibrary.Catcher
{
    abstract class Catcher : ICatcher
    {        
        private Timer timer;

        protected log4net.ILog logger = CrawlerLogger.GetLogger("Catcher");
        protected HttpUtil httpUtil;

        protected IHelper<Proxy> proxyHelper;

        protected bool isUseProxy = false;

        public Catcher(IHelper<Proxy> proxyHelper)
        {
            this.proxyHelper = proxyHelper;
            httpUtil = new HttpUtil();
        }

        public void Start(double interval)
        {
            timer = new Timer(interval);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;

            DoCatch();
        }

        public void Start(double interval, double delay)
        {
            Timer delayTimer = new Timer(delay);
            delayTimer.AutoReset = false;
            delayTimer.Elapsed += (sender, e) =>
            {
                Start(interval);
            };
            delayTimer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                DoCatch();
            }
            catch(Exception ex)
            {
                logger.ErrorFormat("Catcher Timer_Elapsed:{0}", ex);
            }
        }

        private void DoCatch()
        {
            IEnumerable<ICatchItem> catchList = GetCatchList();

            catchList?.ToList().ForEach(async item => 
            {
                if(isUseProxy)
                {
                    var proxy = GetProxy();

                    if(proxy is null)
                    {
                        return;
                    }

                    item.Proxy = GetProxy();
                }

                logger.InfoFormat("{0} 开始抓取: {1}", this.GetType().Name, item.Uri);
                string response = await httpUtil.RequestHTML(item);
                WebResponseHandle(response, item);
            });
        }

        protected Uri GetProxy()
        {
            var list = proxyHelper.Get();

            if(list.Count < 50)
            {
                return null;
            }

            int index = (new Random()).Next(list.Count - 1);
            var proxy = list[index];

            return proxy?.Address;
        }

        protected virtual IEnumerable<ICatchItem> GetCatchList()
        {
            return null;
        }

        protected virtual void WebResponseHandle(string response, ICatchItem catchItem)
        {

        }
    }
}
