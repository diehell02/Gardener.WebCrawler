using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Gardener.WebCrawler.CrawlerLibrary.Rule
{
    interface IPage
    {
        HtmlNodeCollection GetNodes(HtmlNode htmlNode, string ruleName);

        HtmlNode GetSingleNode(HtmlNode htmlNode, string ruleName);

        string GetSingleNodeValue(HtmlNode htmlNode, string ruleName);

        string GetValue(HtmlNode htmlNode, string ruleName);
    }
}
