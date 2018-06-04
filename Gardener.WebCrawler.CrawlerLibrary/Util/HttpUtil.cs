using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using Gardener.WebCrawler.Contracts.Entity;

namespace Gardener.WebCrawler.CrawlerLibrary.Util
{
    public class HttpUtil
    {
        private static log4net.ILog logger = CrawlerLogger.GetLogger("HttpUtil");

        const int DefaultTimeout = 1 * 60 * 1000;

        private Action<RequestState> SyncHttpResponseAction;

        private Uri GetRandomProxy(List<Proxy> list)
        {
            if(list is null)
            {
                return null;
            }

            int index = (new Random()).Next(list.Count - 1);

            return list[index].Address;
        }

        public void AddSyncHttpResponseAction(Action<RequestState> action)
        {
            SyncHttpResponseAction += action;
        }

        public static bool LocalPing(string address)
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(address);

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {

            }            

            return false;
        }

        public async void VerifyProxy(Uri proxyUriString, Action<bool> action = null)
        {
            bool isSuccess = false;
            string response = string.Empty;

            try
            {
                Uri address = new Uri("http://ip.cn/");
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(address);

                if (proxyUriString != null)
                {
                    WebProxy myProxy = new WebProxy(proxyUriString);
                    myHttpWebRequest.Proxy = myProxy;
                }

                myHttpWebRequest.Timeout = 20000;

                myHttpWebRequest.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
                myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063";
                myHttpWebRequest.Host = address.Host;
                myHttpWebRequest.KeepAlive = false;

                myHttpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.6, en; q=0.4, ja; q=0.2");
                myHttpWebRequest.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");

                HttpWebResponse webResponse =  (HttpWebResponse)await myHttpWebRequest.GetResponseAsync();

                if(webResponse.StatusCode == HttpStatusCode.OK)
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Error VerifyProxy proxyUri:{0}, message:{1}", proxyUriString.AbsoluteUri, ex.Message);
            }

            if (action != null)
            {
                action.Invoke(isSuccess);
            }
        }

        public async Task<string> RequestHTML(ICatchItem catchItem)
        {
            string response = string.Empty;

            if (catchItem is null)
            {
                return response;
            }

            Uri address = catchItem.Uri;
            Uri proxy = catchItem.Proxy;
            object extend = catchItem.Extend;
            string cookie = catchItem.Cookie;

            try
            {
                var webCrawlerHttpClientHandler = new WebCrawlerHttpClientHandler(cookie, proxy);
                HttpClient httpClient = new HttpClient(webCrawlerHttpClientHandler);

                response = await httpClient.GetStringAsync(address);
            }
            catch(Exception ex)
            {
                logger.DebugFormat("Error RequestHTML address:{0}: {1}", address.AbsoluteUri, ex.Message);
            }

            return response;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="folderName"></param>
        public async void DownLoad(String requestUrl, string fileName, List<Proxy> proxyList = null, Action<bool> completeAction = null)
        {
            try
            {
                bool isSuccess = false;

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);

                Uri proxyUriString = GetRandomProxy(proxyList);
                if (proxyUriString != null)
                {
                    WebProxy myProxy = new WebProxy(proxyUriString);
                    myHttpWebRequest.Proxy = myProxy;
                }

                myHttpWebRequest.KeepAlive = false;

                HttpWebResponse webResponse = (HttpWebResponse) await myHttpWebRequest.GetResponseAsync();

                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    var stream = webResponse.GetResponseStream();
                    var fs = File.Create(fileName);

                    try
                    {
                        int count = 0;
                        do
                        {
                            var buffer = new byte[4096];
                            count = stream.Read(buffer, 0, buffer.Length);
                            fs.Write(buffer, 0, count);
                        } while (count > 0);

                        if (webResponse.ContentType == "image/jpeg")
                        {
                            // 检查图片完整性
                            fs.Position = fs.Length - 2;
                            int lastByte0 = fs.ReadByte();
                            int lastByte1 = fs.ReadByte();

                            if (lastByte0 == 0xff && lastByte1 == 0xd9)
                            {
                                logger.InfoFormat("图片完整:{0}", requestUrl);
                                isSuccess = true;
                            }
                        }
                        else
                        {
                            isSuccess = true;
                        }
                    }
                    catch(Exception ex)
                    {
                        logger.ErrorFormat("保存图片错误 requestUrl:{0}: {1}", requestUrl, ex.Message);
                    }

                    fs.Close();
                    stream.Close();

                    if(!isSuccess)
                    {
                        File.Delete(fileName);
                    }
                }

                webResponse.Close();

                if (completeAction != null)
                {
                    completeAction.Invoke(isSuccess);
                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Error DownLoad requestUrl:{0}: {1}", requestUrl, ex.Message);
                if (completeAction != null)
                {
                    completeAction.Invoke(false);
                }
            }
        }

        private bool CheckPathExists(string path, string fileName)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return File.Exists(fileName);
            }
            catch(Exception ex)
            {
                logger.ErrorFormat("HttpUtil CheckPathExists:{0}", ex.Message);

                return true;
            }            
        }

        private bool IsJpeg(string fileName)
        {
            bool result = false;

            try
            {
                if(fileName.EndsWith(".jpg"))
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
            catch(Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }
    }

    public class RequestState
    {
        private Uri address;
        public Uri Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        private Uri proxyAddress;
        public Uri ProxyAddress
        {
            get
            {
                return proxyAddress;
            }
            set
            {
                proxyAddress = value;
            }
        }

        private StringBuilder responseData;
        public StringBuilder ResponseData
        {
            get
            {
                return responseData;
            }
        }

        private object extend;
        public object Extend
        {
            get
            {
                return extend;
            }
            set
            {
                extend = value;
            }
        }

        public RequestState()
        {
            responseData = new StringBuilder(5000);
        }
    }

    class WebCrawlerHttpClientHandler : System.Net.Http.HttpClientHandler
    {
        private string cookie = string.Empty;

        public WebCrawlerHttpClientHandler(string cookie = "", Uri proxy = null)
        {
            this.cookie = cookie;
            if(proxy != null)
            {
                this.Proxy = new WebProxy(proxy);
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            request.Headers.Add("Host", request.RequestUri.Host);

            if (!string.IsNullOrEmpty(this.cookie))
            {
                request.Headers.Add("Cookie", this.cookie);
            }

            request.Method = HttpMethod.Get;

            this.ClientCertificateOptions = ClientCertificateOption.Automatic;

            this.AutomaticDecompression = System.Net.DecompressionMethods.GZip;

            return base.SendAsync(request, cancellationToken);
        }
    }
}