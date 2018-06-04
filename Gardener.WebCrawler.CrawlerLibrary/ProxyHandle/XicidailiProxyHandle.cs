using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.Contracts.Entity;
using HtmlAgilityPack;

namespace Gardener.WebCrawler.CrawlerLibrary.ProxyHandle
{
    class XicidailiProxyHandle : IProxyHandle
    {
        private static log4net.ILog logger = CrawlerLogger.GetLogger("XicidailiProxyHandle");

        public XicidailiProxyHandle()
        {

        }

        public List<Proxy> DoHandle(string response)
        {
            try
            {
                List<Proxy> proxyList = new List<Proxy>();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response);

                var rows = doc.DocumentNode.SelectNodes("//table[@id='ip_list']/tr");

                logger.InfoFormat("获取代理列表: 来源:{0} 个数:{1}", "xicidaili", rows.Count);

                foreach (HtmlNode row in rows)
                {
                    string ip = string.Empty;
                    string port = string.Empty;
                    AnonymousType anonymous = AnonymousType.None;
                    ProtocolsType type = ProtocolsType.None;
                    Proxy proxy = null;

                    var cols = row.Descendants().Where(node =>
                    {
                        if (node.Name == "td")
                        {
                            return true;
                        }

                        return false;
                    }).ToList();

                    if (cols.Count != 10)
                    {
                        continue;
                    }

                    var anonymousNode = cols[4];
                    if (anonymousNode != null && anonymousNode.InnerHtml == "高匿")
                    {
                        anonymous = AnonymousType.Elite;
                    }
                    else
                    {
                        continue;
                    }

                    var ipNode = cols[1];
                    Regex regIp = new Regex(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}");
                    if (ipNode != null && regIp.IsMatch(ipNode.InnerHtml))
                    {
                        ip = ipNode.InnerHtml;
                    }
                    else
                    {
                        continue;
                    }

                    var portNode = cols[2];
                    Regex regPort = new Regex(@"\d{2,5}");
                    if (portNode != null && regPort.IsMatch(portNode.InnerHtml))
                    {
                        port = portNode.InnerHtml;
                    }
                    else
                    {
                        continue;
                    }

                    var typeNode = cols[5];
                    if (typeNode != null)
                    {
                        switch (typeNode.InnerHtml)
                        {
                            case "HTTPS":
                                type = ProtocolsType.Both;
                                break;
                            case "HTTP":
                                type = ProtocolsType.Http;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    proxy = new Proxy()
                    {
                        Ip = ip,
                        Port = port,
                        Anonymous = anonymous,
                        Type = type,
                        FlushTime = 0
                    };

                    proxyList.Add(proxy);
                }

                return proxyList;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
    }
}
