using System;
using System.Collections.Generic;
using System.Text;

namespace Gardener.WebCrawler.CrawlerLibrary.Rule
{
    interface IPageRule
    {
        IPage GetRule(PageType pageType);
    }
}
