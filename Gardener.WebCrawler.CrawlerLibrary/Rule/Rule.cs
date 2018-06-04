using System;
using System.Collections.Generic;
using System.Text;

namespace Gardener.WebCrawler.CrawlerLibrary.Rule
{
    class Rule
    {
        public string Name { get; set; }

        public string XPath { get; set; }

        public RuleFun Fun { get; set; }

        public string Param { get; set; }

        public enum RuleFun
        {
            Nodes = 0,
            Node = 1,
            Attr = 2,
            Text = 3,
        }
    }

    enum PageType
    {
        Images = 0,
        AllWork = 1,
        Discover = 2,
        AllPre = 3,
        TopPost = 4,
    }
}
