using System;
using System.Collections.Generic;
using System.Text;
using Gardener.WebCrawler.DataAccessLibrary.Context;
using Gardener.WebCrawler.Contracts.Data;
using Microsoft.EntityFrameworkCore;

namespace Gardener.WebCrawler.DataAccessLibrary.Data
{
    public class BaseHelper<T> : IHelper<T> where T : class
    {
        //private static BaseHelper<T> instance;
        //public static BaseHelper<T> Instance
        //{
        //    get
        //    {
        //        if (instance is null)
        //        {
        //            instance = new BaseHelper<T>();
        //        }

        //        return instance;
        //    }
        //}

        protected virtual DbSet<T> GetDbSet(CrawlerContext crawlerContext)
        {
            return null;
        }

        private TResult Execute<TResult>(Func<CrawlerContext, TResult> func)
        {
            try
            {
                TResult result = default(TResult);

                if (func is null)
                {
                    return result;
                }

                using (var context = new CrawlerContext())
                {
                    result = func.Invoke(context);
                }

                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void Execute(Func<DbSet<T>, T, bool> func, T arg, bool isSaveChange = false)
        {
            try
            {
                if(func is null)
                {
                    return;
                }

                if(arg is T)
                {
                    Execute((crawlerContext) =>
                    {
                        var dbSet = GetDbSet(crawlerContext);

                        bool result = func.Invoke(dbSet, arg);

                        if (result && isSaveChange)
                        {
                            crawlerContext.SaveChanges();
                        }

                        return result;
                    });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private TResult Execute<TResult>(Func<DbSet<T>, TResult> func)
        {
            try
            {
                TResult result = default(TResult);

                if (func is null)
                {
                    return result;
                }

                result = Execute((crawlerContext) =>
                {
                    var dbSet = GetDbSet(crawlerContext);

                    return func.Invoke(dbSet);
                });

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Add(T item)
        {
            if(item is T)
            {
                Execute(Add, item, true);
            }
        }

        protected virtual bool Add(DbSet<T> dbSet, T arg)
        {
            dbSet.Add(arg);

            return true;
        }

        public void Remove(T item)
        {
            if (item is T)
            {
                Execute(Remove, item, true);
            }
        }

        protected virtual bool Remove(DbSet<T> dbSet, T arg)
        {
            dbSet.Remove(arg);

            return false;
        }

        public List<T> Get()
        {
            return Execute(Get);
        }

        protected virtual List<T> Get(DbSet<T> dbSet)
        {
            return null;
        }
    }
}
