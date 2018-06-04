using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Gardener.WebCrawler.Contracts.Entity
{
    public class AllWorkPage : ICatchItem
    {
        public AllWorkPage(int page)
        {
            uri = new Uri(string.Format("https://bcy.net/coser/allwork?&p={0}", page));
        }

        public AllWorkPage(Uri uri)
        {
            this.uri = uri;
        }

        public AllWorkPage(string uriString)
        {
            uri = new Uri(uriString);
        }

        private Uri uri;
        public Uri Uri
        {
            get
            {
                return uri;
            }
        }

        public Uri Proxy
        {
            get;
            set;
        }

        public object Extend
        {
            get;
            set;
        }

        public string Cookie
        {
            get;
            set;
        }
    }
}
