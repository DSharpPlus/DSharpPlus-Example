using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace DSharpPlus_Example
{
    class Program
    {
        public static DiscordClient client;
        public static string token;
        static void Main(string[] args)
        {
            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("Please create a file called 'token.txt' and paste your token in it!");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            token = File.ReadAllText("token.txt");

            // First we'll want to initialize our DiscordClient..
            client = new DiscordClient(new DiscordConfig()
            {
                AutoReconnect = true, // Whether you want DSharpPlus to automatically reconnect
                DiscordBranch = Branch.Stable, // API branch you want to use. Stable is recommended!
                LargeThreshold = 250, // Total number of members where the gateway will stop sending offline members in the guild member list
                LogLevel = LogLevel.Unnecessary, // Minimum log level you want to use
                Token = token, // Your token
                TokenType = TokenType.Bot, // Your token type. Most likely "Bot"
                UseInternalLogHandler = true, // Whether you want to use the internal log handler
            });

            // Now we'll want to define our events
            client.DebugLogger.LogMessage(LogLevel.Info, "Bot", "Initializing events", DateTime.Now);

            // First off, the MessageCreated event.
            client.DebugLogger.LogMessage(LogLevel.Info, "Bot", "Initializing MessageCreated", DateTime.Now);

            client.MessageCreated += async (e) =>
            {
                // We DON'T want to respond to other bots, do we?
                if (!e.Message.Author.IsBot)
                {
                    // A simple ping- pong command
                    if (e.Message.Content == "//ping")
                        await e.Message.Respond("pong");

                    // Attaching a file (make sure image.jpg exists in bot's folder!)
                    if (e.Message.Content == "//image")
                    {
                        // I've used a cat image for this, obviously
                        await e.Message.Respond("meow!", "image.jpg", "image.jpg");
                    }

                    // Attaching an embed
                    if (e.Message.Content == "//embed")
                    {
                        DiscordEmbed embed = new DiscordEmbed()
                        {
                            // "Author" field for our Embed.
                            Author = new DiscordEmbedAuthor()
                            {
                                IconUrl = e.Message.Author.AvatarUrl,
                                Name = e.Message.Author.Username + "#" + e.Message.Author.Discriminator.ToString().PadLeft(4, '0')
                            },

                            // Integer color for our Embed. We'll use a nice dark red color here.
                            Color = 0x460707,

                            // Our embed's description
                            Description = "A very nice example embed!",

                            // Our embed's fields. You can add multiple!
                            Fields = new List<DiscordEmbedField>()
                        {
                            new DiscordEmbedField()
                            {
                                Name = "An example field!",
                                Value = "Very nice indeed!"
                            }
                        },

                            // Our embed's footer.
                            Footer = new DiscordEmbedFooter()
                            {
                                Text = "Wow, nice footer!",
                                IconUrl = "https://s-media-cache-ak0.pinimg.com/564x/01/25/dc/0125dc547e4f43d6493aca279b464895.jpg"
                            },

                            // Our embed's image.
                            Image = new DiscordEmbedImage()
                            {
                                Url = "https://s-media-cache-ak0.pinimg.com/564x/01/25/dc/0125dc547e4f43d6493aca279b464895.jpg"
                            },

                            // Our embed's thumbnail
                            Thumbnail = new DiscordEmbedThumbnail()
                            {
                                Url = "https://s-media-cache-ak0.pinimg.com/564x/01/25/dc/0125dc547e4f43d6493aca279b464895.jpg"
                            },

                            // A timestamp you want to add
                            Timestamp = DateTime.UtcNow,

                            // Our embed's title
                            Title = "An example embed!",

                            // Link URL for our embed
                            Url = "https://s-media-cache-ak0.pinimg.com/564x/01/25/dc/0125dc547e4f43d6493aca279b464895.jpg",
                        };

                        // And now send it c:
                        await e.Message.Respond("Example embed", embed: embed);
                    }

                    // This is how we check whether a channel is private
                    if (e.Message.Content == "//private")
                    {
                        if (e.Channel.IsPrivate)
                            await e.Message.Respond("This is a private channel!");
                        else
                            await e.Message.Respond("This is **not** a private channel!");
                    }

                    // Lets try to update our avatar
                    if (e.Message.Content == "//updateavatar")
                    {
                        await client.SetAvatar("avatar.png");
                    }
                }
            };

            // ChannelCreated event
            client.DebugLogger.LogMessage(LogLevel.Info, "Bot", "Initializing ChannelCreated", DateTime.Now);

            client.ChannelCreated += async (e) =>
            {
                // Lets send a message when a new channel gets created!
                if (e.Channel.Type == ChannelType.Text && !e.Channel.IsPrivate)
                    await e.Channel.SendMessage("Nice! a new channel has been created!");
            };

            // Last but not least, the ready event
            client.DebugLogger.LogMessage(LogLevel.Info, "Bot", "Initializing Ready", DateTime.Now);

            client.Ready += async () =>
            {
                await Task.Yield(); // f*ck the green squiggly lines :^)
                client.DebugLogger.LogMessage(LogLevel.Info, "Bot", "Ready! Setting status message..", DateTime.Now);
                // I recommend setting your playing status in this event.
                await client.UpdateStatus("DSharpPlus-Example by Naamloos");
            };

            // Let's connect!
            client.DebugLogger.LogMessage(LogLevel.Info, "Bot", "Connecting..", DateTime.Now);
            client.Connect();

            // Make sure to not automatically close down
            while (true)
                Console.ReadLine();
        }
    }
}
