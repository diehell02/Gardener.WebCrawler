using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gardener.WebCrawler.Contracts.Entity
{
    [Table("t_Proxy")]
    public class Proxy
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

        public AnonymousType Anonymous
        {
            get;
            set;
        }

        public ProtocolsType Type
        {
            get;
            set;
        }

        public long FlushTime
        {
            get;
            set;
        }

        private Uri address;
        [NotMapped]
        public Uri Address
        {
            get
            {
                if(address == null)
                {
                    StringBuilder _address = new StringBuilder(Ip.Length + Port.Length + 7);

                    _address.Append("http://").Append(Ip).Append(":").Append(Port);

                    address = new Uri(_address.ToString());
                }

                return address;
            }
        }
    }

    public enum AnonymousType
    {
        None = -1,
        Elite = 0,
        Anonymous = 1,
        Transparent = 2
    }

    public enum ProtocolsType
    {
        None = -1,
        Http = 0,
        Https = 1,
        Both = 2,
        Socket4 = 3,
        Socket5 = 4
    }
}
