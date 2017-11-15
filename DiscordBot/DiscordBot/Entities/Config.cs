using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using System.IO;

namespace DiscordBot.Entities
{
    internal class Config
    {
        /// <summary>
        /// Your bot's token.
        /// </summary>
        [JsonProperty("token")]
        internal string Token = "Your token..";

        /// <summary>
        /// Your bot's prefix
        /// </summary>
        [JsonProperty("prefix")]
        internal string Prefix = "'";

        /// <summary>
        /// Your favourite color.
        /// </summary>
        [JsonProperty("color")]
        private string _color = "#7289DA";

        /// <summary>
        /// Your favourite color exposed as a DiscordColor object.
        /// </summary>
        internal DiscordColor Color => new DiscordColor(_color);

        /// <summary>
        /// Loads config from a JSON file.
        /// </summary>
        /// <param name="path">Path to your config file.</param>
        /// <returns></returns>
        public static Config LoadFromFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// Saves config to a JSON file.
        /// </summary>
        /// <param name="path"></param>
        public void SaveToFile(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.Write(JsonConvert.SerializeObject(this));
            }
        }
    }
}
