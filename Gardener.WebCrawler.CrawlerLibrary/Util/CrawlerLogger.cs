using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using log4net;
using log4net.Repository;
using log4net.Config;

namespace Gardener.WebCrawler.CrawlerLibrary.Util
{
    class CrawlerLogger
    {
        static ILoggerRepository repository = null;

        public static ILog GetLogger(string loggerName)
        {
            if (repository is null)
            {
                repository = LogManager.CreateRepository("CrawlerRepository");
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            }

            ILog logger = LogManager.GetLogger(repository.Name, loggerName);

            return logger;
        }

        public static void Flush()
        {    //刷log到文件
            LogManager.Flush(1000);
        }
    }
}
