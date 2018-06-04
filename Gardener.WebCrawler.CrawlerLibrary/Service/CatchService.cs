using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.Contracts.Data;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.CrawlerLibrary.Catcher;

namespace Gardener.WebCrawler.CrawlerLibrary.Service
{
    class CatchService
    {
        const int PROXY_CATCHER_INTERVAL = 900000;
        const int WORK_CATCHER_INTERVAL = 600000;
        const int ALLWORK_CATCHER_INTERVAL = 60000;
        const int WORK_IMAGE_CATCHER_INTERVAL = 60000;
        const int WORK_IMAGE_CATCHER_DELAY = 30000;

        private static log4net.ILog logger = CrawlerLogger.GetLogger("CatchService");

        private ProxyCatcher proxyCatcher;
        private WorkCatcher workCatcher;
        private AllWorkCatcher allWorkCatcher;
        private WorkImageCatcher workImageCatcher;

        Action<WorkImage> syncWorkImageAdd;

        public CatchService(IHelper<ProxyWebApi> proxyWebApiHelper, IHelper<Proxy> proxyHelper,
            IHelper<Work> workHelper, IHelper<WorkImage> workImageHelper, IHelper<Author> authorHelper)
        {
            ProxyCatch(proxyWebApiHelper, proxyHelper);
            WorkCatch(proxyHelper, workHelper, workImageHelper, authorHelper);
        }

        public void AddSync(Action<WorkImage> syncWorkImageAdd)
        {
            this.syncWorkImageAdd += syncWorkImageAdd;
        }

        private void ProxyCatch(IHelper<ProxyWebApi> proxyWebApiHelper, IHelper<Proxy> proxyHelper)
        {
            proxyCatcher = new ProxyCatcher(proxyWebApiHelper, proxyHelper);
            proxyCatcher.Start(PROXY_CATCHER_INTERVAL);
        }

        private void WorkCatch(IHelper<Proxy> proxyHelper, IHelper<Work> workHelper, IHelper<WorkImage> workImageHelper, IHelper<Author> authorHelper)
        {
            workCatcher = new WorkCatcher(proxyHelper, workHelper, authorHelper);
            allWorkCatcher = new AllWorkCatcher(proxyHelper, workHelper, authorHelper);
            workImageCatcher = new WorkImageCatcher(proxyHelper, workHelper, workImageHelper);
            workImageCatcher.AddSync(workImage =>
            {
                syncWorkImageAdd?.Invoke(workImage);
            });

            workCatcher.Start(WORK_CATCHER_INTERVAL);
            allWorkCatcher.Start(ALLWORK_CATCHER_INTERVAL);
            workImageCatcher.Start(WORK_IMAGE_CATCHER_INTERVAL, WORK_IMAGE_CATCHER_DELAY);
        }
    }
}
