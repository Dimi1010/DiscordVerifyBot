using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

using DiscordVerifyBot.Resources;

namespace DiscordVerifyBot.Core.Handlers
{
    /// <summary>
    /// Settings JSON Data Handler
    /// </summary>
    public class SettingsDataHandler : IDisposable
    {
        private readonly string _path;

        public SettingsDataHandler()
        {
            string AssemblyFullPath = Assembly.GetEntryAssembly().Location;
            string AssemblyFilename = Path.GetFileName(AssemblyFullPath);

            //Path to the Settings
            string SettingsPath = AssemblyFullPath.Replace(AssemblyFilename, @"Data\");
            //Name of the Settings
            string SettingsFilename = "settings.json";

            _path = SettingsPath + SettingsFilename;
        }

        /// <summary>
        /// Gets the current settings.json content
        /// </summary>
        /// <returns>Current Settings</returns>
        public Settings GetSettings()
        {
            try
            {
                string jsonFromFile;
                using (var reader = new StreamReader(_path))
                {
                    jsonFromFile = reader.ReadToEnd();
                }
                var settingsFromJson = JsonConvert.DeserializeObject<Settings>(jsonFromFile);
                return settingsFromJson;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the current settings.json content asynchronously
        /// </summary>
        /// <returns>Current Settings</returns>
        public async Task<Settings> GetSettingsAsync()
        {
            try
            {
                string jsonFromFile;
                using (var reader = new StreamReader(_path))
                {
                    jsonFromFile = await reader.ReadToEndAsync();
                }
                var settingsFromJson = JsonConvert.DeserializeObject<Settings>(jsonFromFile);
                return settingsFromJson;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Resets the settings.json to its default values
        /// </summary>
        /// <returns></returns>
        public async Task SaveStockSettings()
        {
            try
            {
                var settings = new Settings
                {
                    BotToken = "",
                    LogLevel = 3,
                    DefaultPrefix = "--"
                };

                var jsonToWrite = JsonConvert.SerializeObject(settings, Formatting.Indented);

                using (var writer = new StreamWriter(_path))
                {
                    await writer.WriteAsync(jsonToWrite);
                }
            }
            catch
            {
                throw;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SettingsDataHandler()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
