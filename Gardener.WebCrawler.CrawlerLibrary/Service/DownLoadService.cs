using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using System.Text.RegularExpressions;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.Contracts.Data;
using Gardener.WebCrawler.Contracts.Entity;

namespace Gardener.WebCrawler.CrawlerLibrary.Service
{
    class DownLoadService
    {
        private System.Timers.Timer timer;
        private System.Timers.Timer syncListTimer;
        private HttpUtil httpUtil;
        private IHelper<DownLoadFile> downLoadHelper;
        private IHelper<Proxy> proxyHelper;

        private bool isLock = false;
        private List<DownLoadFile> downLoadFiles;

        private static log4net.ILog logger = CrawlerLogger.GetLogger("DownLoadService");  

        public DownLoadService(IHelper<DownLoadFile> downLoadHelper, IHelper<Proxy> proxyHelper)
        {
            httpUtil = new HttpUtil();
            this.downLoadHelper = downLoadHelper;
            this.proxyHelper = proxyHelper;

            downLoadFiles = new List<DownLoadFile>();

            timer = new System.Timers.Timer(5000) { AutoReset = true, Enabled = true };
            timer.Elapsed += Timer_Elapsed;

            syncListTimer = new System.Timers.Timer(30000) { AutoReset = true, Enabled = true };
            syncListTimer.Elapsed += SyncListTimer_Elapsed;
        }

        private void SyncListTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<DownLoadFile> _downLoadFiles = downLoadHelper.Get();

            _downLoadFiles.ForEach(downLoadFile =>
            {
                if (!downLoadFiles.Contains(downLoadFile))
                {
                    Wait();
                    downLoadFiles.Add(downLoadFile);
                }                
            });
        }

        public void Add(string address, string fileName)
        {
            downLoadHelper.Add(new DownLoadFile() { Address = address, FileName = fileName });
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                isLock = true;

                downLoadFiles.ForEach(downLoadFile =>
                {
                    if (!downLoadFile.DownLoading)
                    {
                        DownLoad(downLoadFile);
                    }
                });
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
            
            isLock = false;
        }

        private void DownLoad(DownLoadFile downLoadFile)
        {
            try
            {
                List<Proxy> proxyList = proxyHelper.Get();

                if (proxyList is null)
                {
                    return;
                }

                string url = downLoadFile.Address;
                string folderName = downLoadFile.FileName;

                if (string.IsNullOrEmpty(folderName))
                {
                    folderName = "Unknown";
                }

                logger.InfoFormat("开始下载: {0}, {1}", url, folderName);
                downLoadFile.DownLoading = true;

                StringBuilder folderPath = new StringBuilder(folderName.Length + 15);
                StringBuilder filePath = new StringBuilder(folderName.Length + 37);

                string pattern = string.Empty;
                Regex regex = null;

                // 删除尾部限定大小
                regex = new Regex(@"((http|https)://)(([a-zA-Z0-9\._-]+)/)+(w\d+)");
                if (regex.IsMatch(url))
                {
                    url = url.Substring(0, url.LastIndexOf('/'));
                }

                // 拼接生成文件路径
                folderPath.Append("DownLoad/coser/").Append(folderName);
                regex = new Regex(@"((http|https)://)(([a-zA-Z0-9\._-]+)/)+");
                filePath.Append("DownLoad/coser/").Append(folderName).Append('/').Append(url.Substring(regex.Match(url).Value.Length));

                string path = folderPath.ToString();
                string fileName = filePath.ToString();

                bool isExists = false;
                if (FileUtil.CheckPathExists(path, fileName))
                {
                    isExists = true;
                    if (!FileUtil.IsJpeg(fileName))
                    {
                        isExists = false;
                    }
                }

                if (isExists)
                {
                    return;
                }

                httpUtil.DownLoad(url, fileName, proxyList, o =>
                {
                    try
                    {
                        if (o)
                        {
                            logger.InfoFormat("下载完成: {0}, {1}", url, fileName);
                            this.downLoadHelper.Remove(downLoadFile);
                            Wait();
                            downLoadFiles.Remove(downLoadFile);
                        }
                        else
                        {
                            logger.InfoFormat("下载失败: {0}, {1}", url, fileName);
                            downLoadFile.DownLoading = false;
                        }
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex);
                        downLoadFile.DownLoading = false;
                    }
                });
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                downLoadFile.DownLoading = false;
            }
        }

        private void Wait()
        {
            while (isLock)
            {
                Thread.Sleep(10);
            }
        }
    }
}
