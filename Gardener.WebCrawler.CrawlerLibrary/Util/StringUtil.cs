using System;
using System.Collections.Generic;
using System.Text;

namespace Gardener.WebCrawler.CrawlerLibrary.Util
{
    class StringUtil
    {
        public static Uri FillWithDomain(string address, string domain)
        {
            Uri uri = null;

            if(!Uri.TryCreate(address, UriKind.Absolute, out uri))
            {
                if (!Uri.TryCreate(string.Format("{0}{1}", domain, address), UriKind.Absolute, out uri))
                {
                    return null;
                }
            }

            return uri;
        }
    }
}
