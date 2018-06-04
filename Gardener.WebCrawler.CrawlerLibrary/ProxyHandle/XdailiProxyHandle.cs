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
    class XdailiProxyHandle : IProxyHandle
    {
        private static log4net.ILog logger = CrawlerLogger.GetLogger("XicidailiProxyHandle");

        public XdailiProxyHandle()
        {

        }

        public List<Proxy> DoHandle(string response)
        {
            try
            {
                return ProxyHandle(response, jObject =>
                {
                    try
                    {
                        if (jObject == null || jObject["RESULT"] == null || jObject["RESULT"]["rows"] == null)
                        {
                            return null;
                        }

                        JToken RESULT = jObject["RESULT"];

                        logger.InfoFormat("获取代理列表: 来源:{0} 个数:{1}", "xdaili", RESULT["rows"].Count());
                        return RESULT["rows"];
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex);
                        return null;
                    }                    
                }, row =>
                {
                    return CreateProxy(row, item => {
                        return item["ip"];
                    }, item => {
                        return item["port"];
                    }, item => {
                        var anonymousNode = item["anony"];
                        if (anonymousNode != null && anonymousNode.Value<string>() == "高匿")
                        {
                            return AnonymousType.Elite;
                        }
                        else
                        {
                            return AnonymousType.None;
                        }
                    }, item => {
                        var typeNode = item["type"];
                        if (typeNode != null && typeNode.Value<string>() == "HTTP/HTTPS")
                        {
                            return ProtocolsType.Both;
                        }
                        else
                        {
                            return ProtocolsType.None;
                        }
                    });
                });
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
