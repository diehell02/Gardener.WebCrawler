using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Gardener.WebCrawler.CrawlerLibrary.Util
{
    class FileUtil
    {
        private static log4net.ILog logger = CrawlerLogger.GetLogger("FileUtil");

        public static bool CheckPathExists(string path, string fileName)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return File.Exists(fileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                return true;
            }
        }

        public static bool IsJpeg(string fileName)
        {
            bool result = false;

            try
            {
                if (fileName.EndsWith(".jpg"))
                {
                    byte[] bytes = File.ReadAllBytes(fileName);

                    if (bytes[bytes.Length - 2] == 0xff && bytes[bytes.Length - 1] == 0xd9)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }
    }
}
