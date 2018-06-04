using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Gardener.WebCrawler.CrawlerLibrary.Util;
using Gardener.WebCrawler.CrawlerLibrary.ProxyHandle;

namespace Gardener.WebCrawler.CrawlerLibrary.Factory
{
    class ProxyFactory
    {
        protected static log4net.ILog logger = CrawlerLogger.GetLogger("ProxyFactory");

        private IDictionary<string, IProxyHandle> handles = new Dictionary<string, IProxyHandle>();

        private void InitializeFactories()
        {
            Type typeFromHandle = typeof(IProxyHandle);
            Type[] types = typeof(IProxyHandle).Assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (typeFromHandle.IsAssignableFrom(type) && typeFromHandle != type && !type.IsAbstract)
                {
                    try
                    {
                        ConstructorInfo constructor = this.GetConstructor(type);
                        if (constructor != null)
                        {
                            this.handles.Add(string.Intern(type.Name), (IProxyHandle)constructor.Invoke(new object[0]));
                        }
                        else
                        {
                            logger.ErrorFormat("ProxyHandle {0} does not have a suitable constructor", type.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("Unable to construct tracer factory {0} : {1}", type.Name, ex.Message);
                    }
                }
            }
        }

        private ConstructorInfo GetConstructor(Type factoryType)
        {
            Type[][] array = new Type[][]
            {
                new Type[0]
            };
            for (int i = 0; i < array.Length; i++)
            {
                Type[] types = array[i];
                ConstructorInfo constructor = factoryType.GetConstructor(types);
                if (constructor != null)
                {
                    return constructor;
                }
            }
            return null;
        }

        public ProxyFactory()
        {
            InitializeFactories();
        }

        public IProxyHandle GetProxyHandle(string className)
        {
            IProxyHandle handle = null;

            handles.TryGetValue(className, out handle);

            return handle;
        }
    }
}
