using System;
using System.Collections.Generic;
using System.Linq;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.DataAccessLibrary.Context;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Data
{
    public class WorkImageHelper : BaseHelper<WorkImage>
    {
        protected override DbSet<WorkImage> GetDbSet(CrawlerContext crawlerContext)
        {
            return crawlerContext.WorkImages;
        }

        protected override bool Add(DbSet<WorkImage> dbSet, WorkImage arg)
        {
            if (dbSet.Where(item => arg.Address == item.Address).Count() > 0)
            {
                return false;
            }

            return base.Add(dbSet, arg);
        }

        protected override List<WorkImage> Get(DbSet<WorkImage> dbSet)
        {
            return dbSet.ToList();
        }
    }
}
