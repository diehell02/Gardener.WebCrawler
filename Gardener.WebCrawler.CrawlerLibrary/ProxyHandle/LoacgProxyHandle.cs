using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.Contracts.Entity;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Gardener.WebCrawler.CrawlerLibrary.ProxyHandle
{
    class LoacgProxyHandle : IProxyHandle
    {
        private static log4net.ILog logger = CrawlerLogger.GetLogger("LoacgProxyHandle");

        public LoacgProxyHandle()
        {

        }

        public List<Proxy> DoHandle(string response)
        {
            try
            {
                // 处理字符串
                if (response.StartsWith("\"") && response.EndsWith("\""))
                {
                    response = response.Substring(1, response.Length - 2);
                }
                response = response.Replace("\\\"", "\"");

                if (JObject.Parse(response)["code"].Value<int>() == 200)
                {
                    return ProxyHandle(response, jObject =>
                    {
                        logger.InfoFormat("获取代理列表: 来源:{0} 个数:{1}", "loacg", jObject["data"]["data"].Count());
                        return jObject["data"]["data"];
                    }, row =>
                    {
                        return CreateProxy(row, item => {
                            return item["ip"];
                        }, item => {
                            return item["port"];
                        }, item => {
                            return AnonymousType.Elite;
                        }, item => {
                            var type = ProtocolsType.None;
                            var typeNode = item["type"];
                            if (typeNode != null)
                            {
                                switch (typeNode.Value<string>())
                                {
                                    case "0":
                                        type = ProtocolsType.Http;
                                        break;
                                    case "1":
                                        type = ProtocolsType.Https;
                                        break;
                                    case "2":
                                        type = ProtocolsType.Socket4;
                                        break;
                                    case "3":
                                        type = ProtocolsType.Socket5;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            return type;
                        });
                    });
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        private Proxy CreateProxy(JToken item,
            Func<JToken, JToken> selectIpNode,
            Func<JToken, JToken> selectPortNode,
            Func<JToken, AnonymousType> selectAnonymousNode,
            Func<JToken, ProtocolsType> selectTypeNode)
        {
            string ip = string.Empty;
            string port = string.Empty;
            AnonymousType anonymous = AnonymousType.None;
            ProtocolsType type = ProtocolsType.None;
            Proxy proxy = null;

            anonymous = selectAnonymousNode.Invoke(item);
            if (anonymous == AnonymousType.None)
            {
                return null;
            }

            var ipNode = selectIpNode.Invoke(item); ;
            if (ipNode != null)
            {
                ip = ipNode.Value<string>();
            }
            else
            {
                return null;
            }

            var portNode = selectPortNode.Invoke(item); ;
            if (portNode != null)
            {
                port = portNode.Value<string>();
            }
            else
            {
                return null;
            }

            type = selectTypeNode.Invoke(item);
            if (type == ProtocolsType.None)
            {
                return null;
            }

            proxy = new Proxy()
            {
                Ip = ip,
                Port = port,
                Anonymous = anonymous,
                Type = type,
                FlushTime = 0
            };

            return proxy;
        }

        private List<Proxy> ProxyHandle(string response, Func<JObject, JToken> getRows, Func<JToken, Proxy> createProxy)
        {
            List<Proxy> proxyList = new List<Proxy>();

            // 处理字符串
            if (response.StartsWith("\"") && response.EndsWith("\""))
            {
                response = response.Substring(1, response.Length - 2);
            }
            response = response.Replace("\\\"", "\"");

            JObject jObject = JObject.Parse(response);

            var rowNodes = getRows.Invoke(jObject);

            if (rowNodes != null)
            {
                rowNodes.ToList().ForEach(row =>
                {
                    Proxy proxy = createProxy.Invoke(row);

                    proxyList.Add(proxy);
                });
            }

            return proxyList;
        }
    }
}
