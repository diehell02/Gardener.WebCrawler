using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gardener.WebCrawler.Contracts.Entity
{
    [Table("t_Author")]
    public class Author
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public string Introduction
        {
            get;
            set;
        }
    }
}
