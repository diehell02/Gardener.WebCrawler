using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gardener.WebCrawler.Contracts.Entity
{
    [Table("t_Work")]
    public class WorkDetail
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

        public string Content
        {
            get;
            set;
        }

        public List<WorkImage> WorkImages
        {
            get;
            set;
        }
    }
}
