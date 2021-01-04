using Impostor.Api.Plugins;
using Impostor.Api.Events.Managers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using org.visualenterprise.AmongUsDetective.Handlers;

namespace org.visualenterprise.AmongUsDetective {

    [ImpostorPlugin(
        package: "org.visualenterprise.AmongUsDetective",
        name: "AmongUsDetective",
        author: "Siebs",
        version: "1.0.4")]
    public class Detective : PluginBase {

        private readonly ILogger<Detective> _logger;
        private readonly IEventManager _eventManager;
        private IDisposable _unregister;

        public Detective(ILogger<Detective> logger, IEventManager eventManager) {
            _logger = logger;
            _eventManager = eventManager;
        }

        public override ValueTask EnableAsync() {
            _logger.LogInformation("Detective is being enabled");
            _unregister = _eventManager.RegisterListener(new GameEventListener(_logger));
            return default;
        }

        public override ValueTask DisableAsync() {
            _logger.LogInformation("Detective is being disabled");
            _unregister.Dispose();
            return default;
        }

    }
}
