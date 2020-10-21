using DryIoc;

using Shadowsocks.Common.Model;
using Shadowsocks.Controller;
using Shadowsocks.Model;

namespace Shadowsocks
{
    public static class ShadowsocksClient
    {
        static ShadowsocksClient()
        {
            var container = IoCManager.Container;

            // register bean
            container.Register<Configuration>(Reuse.Singleton);

            // register service
            //container.Register<IService, GeositeUpdaterService>(Reuse.Singleton);
        }

        public static void Startup()
        {
            InitServices();
        }

        private static void InitServices()
        {
            var services = IoCManager.Container.ResolveMany<IService>();

            foreach (var service in services)
                service.Startup();
        }
    }
}
