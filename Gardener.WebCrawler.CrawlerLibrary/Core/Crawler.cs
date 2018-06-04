using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gardener.WebCrawler.CrawlerLibrary.Service;
using Gardener.WebCrawler.Contracts.Data;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.CrawlerLibrary.Util;

namespace Gardener.WebCrawler.CrawlerLibrary.Core
{
    public class Crawler
    {
        static CatchService catchService;
        static DownLoadService downLoadService;

        private static log4net.ILog logger = CrawlerLogger.GetLogger("Catcher");

        public static void Init(IHelper<ProxyWebApi> proxyWebApiHelper, IHelper<Proxy> proxyHelper,
            IHelper<Work> workHelper, IHelper<WorkImage> workImageHelper, IHelper<Author> authorHelper, 
            IHelper<DownLoadFile> downLoadHelper)
        {
            logger.Info("Crawler Init.");

            catchService = new CatchService(proxyWebApiHelper, proxyHelper, workHelper, workImageHelper, authorHelper);
            //downLoadService = new DownLoadService(downLoadHelper, proxyHelper);

            catchService.AddSync(workImage =>
            {
                Work work = workImage.Work;
                string coserName = work.Name;
                string title = work.Title;
                string fileName = string.Empty;

                if (!string.IsNullOrEmpty(coserName))
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        fileName = string.Format("{0}/{1}", coserName, title);
                    }
                    else
                    {
                        fileName = coserName;
                    }
                }
                else
                {
                    fileName = "Unknown";
                }

                downLoadService?.Add(workImage.Address, fileName);
            });
        }

        public static void Exit()
        {
            logger.Info("Crawler Exit.");
            CrawlerLogger.Flush();
        }
    }
}
