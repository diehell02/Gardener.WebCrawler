using System;
using Gardener.WebCrawler.CrawlerLibrary.Core;
using Gardener.WebCrawler.DataAccessLibrary.Data;

namespace Gardener.WebCrawler.CrawlerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (Gardener.WebCrawler.DataAccessLibrary.Context.CrawlerContext crawlerContext = new DataAccessLibrary.Context.CrawlerContext())
            //{
            //    var author = new Contracts.Entity.Author() { Address = "123" };

            //    crawlerContext.Authors.Add(author);

            //    crawlerContext.SaveChanges();
            //}



                Crawler.Init(new ProxyWebApiHelper(),
                    new ProxyHelper(),
                    new WorkHelper(),
                    new WorkImageHelper(),
                    new AuthorHelper(),
                    new DownLoadHelper());

            while (true)
            {
                string readLine = Console.ReadLine();

                if (readLine == "exit")
                {
                    break;
                }
            }
        }
    }
}
