using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot.Entities;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using System.Threading;

namespace DiscordBot
{
    public class Bot : IDisposable
    {
        private DiscordClient _client;
        private InteractivityModule _interactivity;
        private CommandsNextModule _cnext;
        private Config _config;
        private StartTimes _starttimes;
        private CancellationTokenSource _cts;

        public Bot()
        {
            if (!File.Exists("config.json"))
            {
                new Config().SaveToFile("config.json");
                #region !! Report to user that config has not been set yet !! (aesthetics)
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                WriteCenter("▒▒▒▒▒▒▒▒▒▄▄▄▄▒▒▒▒▒▒▒", 2);
                WriteCenter("▒▒▒▒▒▒▄▀▀▓▓▓▀█▒▒▒▒▒▒");
                WriteCenter("▒▒▒▒▄▀▓▓▄██████▄▒▒▒▒");
                WriteCenter("▒▒▒▄█▄█▀░░▄░▄░█▀▒▒▒▒");
                WriteCenter("▒▒▄▀░██▄░░▀░▀░▀▄▒▒▒▒");
                WriteCenter("▒▒▀▄░░▀░▄█▄▄░░▄█▄▒▒▒");
                WriteCenter("▒▒▒▒▀█▄▄░░▀▀▀█▀▒▒▒▒▒");
                WriteCenter("▒▒▒▄▀▓▓▓▀██▀▀█▄▀▀▄▒▒");
                WriteCenter("▒▒█▓▓▄▀▀▀▄█▄▓▓▀█░█▒▒");
                WriteCenter("▒▒▀▄█░░░░░█▀▀▄▄▀█▒▒▒");
                WriteCenter("▒▒▒▄▀▀▄▄▄██▄▄█▀▓▓█▒▒");
                WriteCenter("▒▒█▀▓█████████▓▓▓█▒▒");
                WriteCenter("▒▒█▓▓██▀▀▀▒▒▒▀▄▄█▀▒▒");
                WriteCenter("▒▒▒▀▀▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒");
                Console.BackgroundColor = ConsoleColor.Yellow;
                WriteCenter("WARNING", 3);
                Console.ResetColor();
                WriteCenter("Thank you Mario!", 1);
                WriteCenter("But our config.json is in another castle!");
                WriteCenter("(Please fill in the config.json that was generated.)", 2);
                WriteCenter("Press any key to exit..", 1);
                Console.SetCursorPosition(0, 0);
                Console.ReadKey();
                #endregion
                Environment.Exit(0);
            }
            this._config = Config.LoadFromFile("config.json");
            _client = new DiscordClient(new DiscordConfiguration()
            {
                AutoReconnect = true,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = _config.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            _interactivity = _client.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehaviour = TimeoutBehaviour.Delete,
                PaginationTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(30)
            });

            _starttimes = new StartTimes()
            {
                BotStart = DateTime.Now,
                SocketStart = DateTime.MinValue
            };

            _cts = new CancellationTokenSource();

            DependencyCollection dep = null;
            using (var d = new DependencyCollectionBuilder())
            {
                d.AddInstance(new Dependencies()
                {
                    Interactivity = this._interactivity,
                    StartTimes = this._starttimes,
                    Cts = this._cts
                });
                dep = d.Build();
            }

            _cnext = _client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                StringPrefix = _config.Prefix,
                IgnoreExtraArguments = true,
                Dependencies = dep
            });

            _cnext.RegisterCommands<Commands.Owner>();
            _cnext.RegisterCommands<Commands.Interactivity>();

            _client.Ready += OnReadyAsync;
        }

        public async Task RunAsync()
        {
            await _client.ConnectAsync();
            await WaitForCancellationAsync();
        }

        private async Task WaitForCancellationAsync()
        {
            while(!_cts.IsCancellationRequested)
                await Task.Delay(500);
        }

        private async Task OnReadyAsync(ReadyEventArgs e)
        {
            await Task.Yield();
            _starttimes.SocketStart = DateTime.Now;
        }

        public void Dispose()
        {
            this._client.Dispose();
            this._interactivity = null;
            this._cnext = null;
            this._config = null;
        }

        internal void WriteCenter(string value, int skipline = 0)
        {
            for (int i = 0; i < skipline; i++)
                Console.WriteLine();

            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
        }
    }
}
