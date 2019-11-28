namespace DiscordVerifyBot.Resources
{
    public class Settings
    {
        public string BotToken { get; set; }
        public int LogLevel { get; set; } 
        public int RollingLogRetainedFiles { get; set; }
        public string DefaultPrefix { get; set; }
    }
}
