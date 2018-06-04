using System;
using System.Collections.Generic;
using System.Text;
using Gardener.WebCrawler.Contracts.Entity;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Context
{
    public class CrawlerContext : BaseContext
    {
        public DbSet<Proxy> Proxys
        {
            get;
            set;
        }

        public DbSet<Work> Works
        {
            get;
            set;
        }

        public DbSet<WorkImage> WorkImages
        {
            get;
            set;
        }

        public DbSet<DownLoadFile> DownLoadFiles
        {
            get;
            set;
        }

        public DbSet<ProxyWebApi> ProxyWebApis
        {
            get;
            set;
        }

        public DbSet<Author> Authors
        {
            get;
            set;
        }
    }
}
