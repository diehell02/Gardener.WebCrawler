using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gardener.WebCrawler.Contracts.Entity
{
    [Table("t_WorkImage")]
    public class WorkImage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id
        {
            get;
            set;
        }

        public long WorkId
        {
            get;
            set;
        }

        [NotMapped]
        public Work Work
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }
    }
}
