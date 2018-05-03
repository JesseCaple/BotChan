using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotChan
{
    /// <summary>
    /// Simple strings-only configuration pulled from a property-value file.
    /// </summary>
    class Config
    {
        /// <summary>
        /// The asigned token of the bot.
        /// </summary>
        public string BotToken { get; private set; }

        /// <summary>
        /// Use <see cref="OpenAsync(string)"/> to create a <see cref="Config"/> object.
        /// </summary>
        private Config()
        { }

        /// <summary>
        /// Opens or creates a config file.
        /// </summary>
        /// <param name="path">The path to the file to open or create.</param>
        /// <returns>A <see cref="Config"/> object containing the values pulled from the opened file.</returns>
        public static async Task<Config> OpenAsync(string path)
        {
            if (!File.Exists(path))
            {
                using (var file = File.CreateText(path))
                {
                    foreach (var property in typeof(Config).GetProperties())
                    {
                        await file.WriteLineAsync($"{property.Name}=");
                    }
                }
                throw new MissingConfigurationValueException(typeof(Config).GetProperties().First().Name);
            }

            var type = typeof(Config);
            var config = new Config();
            using (var file = File.OpenText(path))
            {
                while (!file.EndOfStream)
                {
                    var line = await file.ReadLineAsync();
                    if (line.IndexOf('=') > 1 && line.Length > 3)
                    {
                        var name = line.Substring(0, line.IndexOf('=')).Trim();
                        var value = line.Substring(line.IndexOf('=') + 1, line.Length - name.Length - 1).Trim();
                        var property = type.GetProperty(name);
                        if (property != null)
                        {
                            property.SetValue(config, value);
                        }
                    }
                }
            }
            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(config);
                if (value == null || string.IsNullOrEmpty((string)value))
                {
                    throw new MissingConfigurationValueException(property.Name);
                }
            }
            return config;
        }
    }
}
