using cc.ts13.AmongUsDetective.Handlers;
using Impostor.Api.Events;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cc.ts13.AmongUsDetective {
    class DetectiveStartup : IPluginStartup {

        public void ConfigureHost(IHostBuilder host) {
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddSingleton<IEventListener, GameEventListener>();
        }

    }
}
