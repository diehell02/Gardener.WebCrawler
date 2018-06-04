using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gardener.WebCrawler.Contracts.Entity
{
    [Table("t_DownLoadFile")]
    public class DownLoadFile
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

        public string FileName
        {
            get;
            set;
        }

        [NotMapped]
        public bool DownLoading
        {
            get;
            set;
        }
    }
}
