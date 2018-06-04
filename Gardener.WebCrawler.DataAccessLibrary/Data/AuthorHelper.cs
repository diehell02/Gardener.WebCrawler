using System;
using System.Collections.Generic;
using System.Linq;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.DataAccessLibrary.Context;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Data
{
    public class AuthorHelper : BaseHelper<Author>
    {
        protected override DbSet<Author> GetDbSet(CrawlerContext crawlerContext)
        {
            return crawlerContext.Authors;
        }

        protected override bool Add(DbSet<Author> dbSet, Author arg)
        {
            if (dbSet.Where(item => arg.Address == item.Address).Count() > 0)
            {
                return false;
            }

            return base.Add(dbSet, arg);
        }

        protected override List<Author> Get(DbSet<Author> dbSet)
        {
            return dbSet.ToList();
        }
    }
}
