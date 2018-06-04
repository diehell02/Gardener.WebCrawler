using System;
using System.Collections.Generic;
using System.Linq;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.DataAccessLibrary.Context;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Data
{
    public class ProxyWebApiHelper : BaseHelper<ProxyWebApi>
    {
        protected override DbSet<ProxyWebApi> GetDbSet(CrawlerContext crawlerContext)
        {
            return crawlerContext.ProxyWebApis;
        }

        protected override bool Add(DbSet<ProxyWebApi> dbSet, ProxyWebApi arg)
        {
            if (dbSet.Where(item => item.Address == arg.Address).Count() > 0)
            {
                return false;
            }

            return base.Add(dbSet, arg);
        }

        protected override List<ProxyWebApi> Get(DbSet<ProxyWebApi> dbSet)
        {
            return dbSet.ToList();
        }
    }
}
