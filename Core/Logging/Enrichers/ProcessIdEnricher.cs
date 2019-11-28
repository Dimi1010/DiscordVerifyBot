using System.Diagnostics;

using Serilog.Core;
using Serilog.Events;

namespace DiscordVerifyBot.Core.Logging
{
    class ProcessIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ProcessId", Process.GetCurrentProcess().Id.ToString()));
        }
    }
}
