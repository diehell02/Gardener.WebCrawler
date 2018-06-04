using System;
using System.Collections.Generic;
using System.Linq;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.DataAccessLibrary.Context;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Data
{
    public class WorkHelper : BaseHelper<Work>
    {
        protected override DbSet<Work> GetDbSet(CrawlerContext crawlerContext)
        {
            return crawlerContext.Works;
        }

        protected override bool Add(DbSet<Work> dbSet, Work arg)
        {
            if (dbSet.Where(item => arg.Address == item.Address).Count() > 0)
            {
                return false;
            }

            return base.Add(dbSet, arg);
        }

        protected override List<Work> Get(DbSet<Work> dbSet)
        {
            return dbSet.ToList();
        }
    }
}
