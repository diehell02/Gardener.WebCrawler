using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.Contracts.Data;
using Gardener.WebCrawler.Contracts.Entity;
using HtmlAgilityPack;
using Gardener.WebCrawler.CrawlerLibrary.Rule;
using Gardener.WebCrawler.CrawlerLibrary.Config;

namespace Gardener.WebCrawler.CrawlerLibrary.Catcher
{
    class WorkCatcher : Catcher
    {
        IHelper<Work> workHelper;
        IHelper<Author> authorHelper;

        private AllWorkPage allWorkPage;

        public WorkCatcher(IHelper<Proxy> proxyHelper, IHelper<Work> workHelper, IHelper<Author> authorHelper) : base(proxyHelper)
        {
            this.workHelper = workHelper;
            this.authorHelper = authorHelper;

            isUseProxy = true;

            allWorkPage = new AllWorkPage(string.Format("https://bcy.net/coser/allwork?&p={0}", 1));
        }

        protected override IEnumerable<ICatchItem> GetCatchList()
        {
            return new List<ICatchItem>() { allWorkPage };
        }

        protected override void WebResponseHandle(string response, ICatchItem catchItem)
        {
            try
            {
                string requestDomain = string.Empty;
                requestDomain = string.Format("{0}://{1}", catchItem.Uri.Scheme, catchItem.Uri.Host);

                string html = response;
                var doc = new HtmlDocument();

                doc.LoadHtml(html);

                IPage page = RuleConfig.PageRule.GetRule(PageType.AllWork);

                var galleryNodes = page.GetNodes(doc.DocumentNode, "Gallery");
                if (galleryNodes != null)
                {
                    logger.InfoFormat("获取作品列表: 个数:{0}", galleryNodes.Count);

                    galleryNodes.ToList().ForEach(item =>
                    {
                        Work work = new Work();

                        string _address = page.GetSingleNodeValue(item, "Gallery.Address");
                        work.Address = StringUtil.FillWithDomain(_address, requestDomain)?.AbsoluteUri;
                        //work.Originality = page.GetSingleNodeValue(item, "Gallery.Originality");
                        //work.Role = page.GetSingleNodeValue(item, "Gallery.Role");
                        work.Name = page.GetSingleNodeValue(item, "Gallery.Name");

                        Author author = new Author();

                        author.Address = page.GetSingleNodeValue(item, "Gallery.Author.Address");
                        author.Name = work.Name;

                        authorHelper.Add(author);
                        workHelper.Add(work);

                        logger.InfoFormat("获得作品: [Address:{0}], [Originality:{1}], [Role:{2}], [Name:{3}]", work.Address, work.Originality, work.Role, work.Name);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("WorkCatcher WebResponseHandle:{0}", ex);
            }        
        }
    }
}
