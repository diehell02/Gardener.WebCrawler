using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gardener.WebCrawler.Contracts.Entity
{
    [Table("t_ProxyTest")]
    public class ProxyTest
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id
        {
            get;
            set;
        }

        public string Ip
        {
            get;
            set;
        }

        public string Port
        {
            get;
            set;
        }

        public Int64 Anonymous
        {
            get;
            set;
        }
    }
}
