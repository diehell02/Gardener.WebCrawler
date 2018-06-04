using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Gardener.WebCrawler.DataAccessLibrary.Config;

namespace Gardener.WebCrawler.DataAccessLibrary.Context
{
    public class BaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(string.Format("Data Source={0}", DbConfig.DbSource));
        }
    }
}
