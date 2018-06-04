using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using HtmlAgilityPack;
using Gardener.WebCrawler.Contracts.Data;
using Gardener.WebCrawler.Contracts.Entity;
using Gardener.WebCrawler.CrawlerLibrary.Config;
using Gardener.WebCrawler.CrawlerLibrary.Rule;

namespace Gardener.WebCrawler.CrawlerLibrary.Catcher
{
    class WorkImageCatcher : Catcher
    {
        IHelper<Work> workHelper;
        IHelper<WorkImage> workImageHelper;
        
        Action<WorkImage> syncWorkImageAdd;

        public WorkImageCatcher(IHelper<Proxy> proxyHelper,
            IHelper<Work> workHelper, IHelper<WorkImage> workImageHelper):base(proxyHelper)
        {
            this.workHelper = workHelper;
            this.workImageHelper = workImageHelper;

            isUseProxy = true;
        }

        public void AddSync(Action<WorkImage> syncWorkImageAdd)
        {
            this.syncWorkImageAdd += syncWorkImageAdd;
        }

        protected override IEnumerable<ICatchItem> GetCatchList()
        {
            return workHelper.Get();
        }

        protected override void WebResponseHandle(string response, ICatchItem catchItem)
        {
            try
            {
                string html = response;
                var doc = new HtmlDocument();
                string coserName = string.Empty;
                string title = string.Empty;
                string fileName = string.Empty;

                doc.LoadHtml(html);

                IPage page = RuleConfig.PageRule.GetRule(PageType.Images);

                coserName = page.GetSingleNodeValue(doc.DocumentNode, "CoserName");

                title = page.GetSingleNodeValue(doc.DocumentNode, "Title");

                if (!string.IsNullOrEmpty(title))
                {
                    foreach (char rInvalidChar in System.IO.Path.GetInvalidPathChars())
                    {
                        title = title.Replace(rInvalidChar.ToString(), string.Empty);
                    }
                    string errChar = "\\/:*?";
                    foreach (char rInvalidChar in errChar)
                    {
                        title = title.Replace(rInvalidChar.ToString(), string.Empty);
                    }
                }
                
                var imgNodes = page.GetNodes(doc.DocumentNode, "WorkImage");
                if (imgNodes != null)
                {
                    if(catchItem.Extend != null && catchItem.Extend is Work)
                    {
                        Work work = (Work)catchItem.Extend;
                        string workAddress = catchItem.Uri.AbsoluteUri;

                        imgNodes.ToList()
                        .ForEach(x =>
                        {
                            string address = page.GetSingleNodeValue(x, "WorkImage.Src");

                            // 删除尾部限定大小
                            var regex = new Regex(@"((http|https)://)(([a-zA-Z0-9\._-]+)/)+(w\d+)");
                            if (regex.IsMatch(address))
                            {
                                address = address.Substring(0, address.LastIndexOf('/'));
                            }

                            logger.InfoFormat("添加图片:[Address:{0}, CoserName:{1}, WorkAddress:{2}]", address, coserName, workAddress);

                            WorkImage workImage = new WorkImage() { Work = work, WorkId = work.Id, Address = address };
                            workImageHelper.Add(workImage);
                            // 事件推送图片添加成功
                            syncWorkImageAdd?.Invoke(workImage);
                        });
                        // 完成后，从作品抓取列表中清除
                        logger.InfoFormat("完成抓取: {0}", catchItem.Uri.AbsoluteUri);
                        // 事件推送作品图片已抓取
                        work.IsCatchImage = true;
                        work.Title = title;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.ErrorFormat("WorkImageCatcher WebResponseHandle:{0}", ex);
            }
        }
    }
}
