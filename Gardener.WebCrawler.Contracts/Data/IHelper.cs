using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardener.WebCrawler.Contracts.Data
{
    public interface IHelper<T>
    {
        void Add(T t);

        void Remove(T t);

        List<T> Get();
    }
}
