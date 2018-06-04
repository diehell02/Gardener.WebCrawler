using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardener.WebCrawler.CrawlerLibrary.Catcher
{
    public interface ICatcher
    {
        void Start(double interval);

        void Start(double interval, double delay);
    }
}
