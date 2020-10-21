using DryIoc;

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Common.Model
{
    public static class IoCManager
    {
        public static IContainer Container { get; }

        static IoCManager()
        {
            Container = new Container();
        }
    }
}
