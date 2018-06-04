using System;
using System.Collections.Generic;
using System.Linq;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.DataAccessLibrary.Context;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Data
{
    public class ProxyHelper : BaseHelper<Proxy>
    {
        protected override DbSet<Proxy> GetDbSet(CrawlerContext crawlerContext)
        {
            return crawlerContext.Proxys;
        }

        protected override bool Add(DbSet<Proxy> dbSet, Proxy arg)
        {
            if (dbSet.Where(item => item.Ip == arg.Ip && item.Port == arg.Port).Count() > 0)
            {
                return false;
            }

            return base.Add(dbSet, arg);
        }

        protected override List<Proxy> Get(DbSet<Proxy> dbSet)
        {
            return dbSet.ToList();
        }
    }
}
