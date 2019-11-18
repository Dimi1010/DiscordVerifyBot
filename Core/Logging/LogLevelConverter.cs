using System.Collections.Generic;

using Serilog.Events;
using Discord;

namespace DiscordVerifyBot.Core.Logging
{
    class LogLevelConverter
    {
        private readonly Dictionary<LogEventLevel, LogSeverity> serilogToDiscordMap = new Dictionary<LogEventLevel, LogSeverity>();
        private readonly Dictionary<LogSeverity, LogEventLevel> discordToSerilogMap = new Dictionary<LogSeverity, LogEventLevel>();

        public LogLevelConverter()
        {
            serilogToDiscordMap.Add(LogEventLevel.Verbose, LogSeverity.Verbose);    //Verbose
            serilogToDiscordMap.Add(LogEventLevel.Debug, LogSeverity.Debug);        //Debug
            serilogToDiscordMap.Add(LogEventLevel.Information, LogSeverity.Info);   //Information
            serilogToDiscordMap.Add(LogEventLevel.Warning, LogSeverity.Warning);    //Warning
            serilogToDiscordMap.Add(LogEventLevel.Error, LogSeverity.Error);        //Error
            serilogToDiscordMap.Add(LogEventLevel.Fatal, LogSeverity.Critical);     //Fatal
            
            discordToSerilogMap.Add(LogSeverity.Verbose, LogEventLevel.Verbose);    //Verbose
            discordToSerilogMap.Add(LogSeverity.Debug, LogEventLevel.Debug);        //Debug
            discordToSerilogMap.Add(LogSeverity.Info, LogEventLevel.Information);   //Information
            discordToSerilogMap.Add(LogSeverity.Warning, LogEventLevel.Warning);    //Warning
            discordToSerilogMap.Add(LogSeverity.Error, LogEventLevel.Error);        //Error
            discordToSerilogMap.Add(LogSeverity.Critical, LogEventLevel.Fatal);     //Fatal
        }

        public bool SerilogToDiscordNet(LogEventLevel serilogLevel, out LogSeverity discordLevel)
        {
            if (serilogToDiscordMap.TryGetValue(serilogLevel, out discordLevel))
                return true;
            else
                return false;
        }

        public bool DiscordNetToSerilog(LogSeverity discordLevel, out LogEventLevel serilogLevel)
        {
            if (discordToSerilogMap.TryGetValue(discordLevel, out serilogLevel))
                return true;
            else
                return false;
        }
    }
}
