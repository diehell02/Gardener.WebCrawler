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
    [Table("t_Work")]
    public class Work : ICatchItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// 原作
        /// </summary>
        public string Originality
        {
            get;
            set;
        }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role
        {
            get;
            set;
        }

        /// <summary>
        /// coser名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 作品网址
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 作品标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 标题图片
        /// </summary>
        public string Gallery
        {
            get;
            set;
        }

        /// <summary>
        /// 是否已抓取图片列表
        /// </summary>
        public bool IsCatchImage
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
                if (uri is null)
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
                return this;
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
