using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Gardener.WebCrawler.Contracts.Entity
{
    [Table("t_ProxyWebApi")]
    public class ProxyWebApi : ICatchItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public string ClassName
        {
            get;
            set;
        }

        private Uri uri;
        [NotMapped]
        public Uri Uri
        {
            get
            {
                if(uri is null)
                {
                    uri = new Uri(Address);
                }

                return uri;
            }
        }

        [NotMapped]
        public Uri Proxy
        {
            get;
            set;
        }

        [NotMapped]
        public object Extend
        {
            get
            {
                return ClassName;
            }
        }

        [NotMapped]
        public string Cookie
        {
            get;
            set;
        }
    }
}
