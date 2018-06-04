using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gardener.WebCrawler.Contracts.Entity;

namespace Gardener.WebCrawler.Contracts.Api
{
    public interface IWorkApi
    {
        bool Login(string userName, string password);

        List<Work> GetAllWork(int page = 1);

        WorkDetail GetWorkDetail(Work work);
    }
}
