using System;
using System.Collections.Generic;
using System.Linq;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.DataAccessLibrary.Context;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Data
{
    public class DownLoadHelper : BaseHelper<DownLoadFile>
    {
        protected override DbSet<DownLoadFile> GetDbSet(CrawlerContext crawlerContext)
        {
            return crawlerContext.DownLoadFiles;
        }

        protected override bool Add(DbSet<DownLoadFile> dbSet, DownLoadFile arg)
        {
            if (dbSet.Where(item => item.Address != arg.Address).Count() > 0)
            {
                return false;
            }

            return base.Add(dbSet, arg);
        }

        protected override List<DownLoadFile> Get(DbSet<DownLoadFile> dbSet)
        {
            return dbSet.ToList();
        }
    }
}
