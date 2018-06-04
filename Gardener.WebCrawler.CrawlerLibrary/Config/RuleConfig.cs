using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gardener.WebCrawler.CrawlerLibrary.Rule;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Gardener.WebCrawler.CrawlerLibrary.Config
{
    class RuleConfig
    {
        static IPageRule pageRule;
        public static IPageRule PageRule
        {
            get
            {
                return pageRule;
            }
        }

        static RuleConfig()
        {
            using(JsonReader jsonReader = new JsonTextReader(new StreamReader("rule.json")))
            {
                JObject jObject = JObject.Load(jsonReader);

                var jTokens = jObject["Rule"]?.AsJEnumerable();

                if (jTokens != null)
                {
                    var _pageRule = new PageRule();

                    foreach (var jToken in jTokens)
                    {
                        PageType pageType;

                        if(!Enum.TryParse(jToken?.Value<string>("PageType"), out pageType))
                        {
                            continue;
                        }

                        Page page = new Page();

                        var jRules = jToken["Rules"].AsJEnumerable();

                        foreach (var jRule in jRules)
                        {
                            Rule.Rule rule = new Rule.Rule()
                            {
                                Name = jRule?.Value<string>("Name"),
                                XPath = jRule?.Value<string>("XPath"),
                                Fun = Enum.Parse<Rule.Rule.RuleFun>(jRule?.Value<string>("Fun")),
                                Param = jRule?.Value<string>("Param"),
                            };
                            page.Add(rule);
                        }

                        _pageRule.AddRule(pageType, page);
                    }

                    pageRule = _pageRule;
                }
            }
        }
    }
}
