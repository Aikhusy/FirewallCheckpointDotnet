using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using System.Text;

namespace Firewall
{
    public class Notif : INotif
    {
        private ITelegramBotClient botClient;
        private readonly IJsonReader _JsonReader;

        public Notif(IJsonReader jsonReader)
        {
            // Initialize the bot client
            _JsonReader = jsonReader;
            botClient = new TelegramBotClient("6716626405:AAGrbNmB8CUpFSA-r57xS1eRAx2PuX2yZxc");
        }

        // Method that initializes and runs the bot
        public async Task RunBot(StringBuilder messages)
        {
            ITeleBot config = _JsonReader.ReadTelegramJsonConfig("Config.json");

            // Corrected comparison
            if (config.Telegram_Alert_Status == false || config.Telegram_Alert_Status == null || messages.Length == 0)
            {
                return;
            }


            var me = await botClient.GetMeAsync();

            // Start receiving updates
            var cts = new CancellationTokenSource();
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                cancellationToken: cts.Token
            );

            await botClient.SendTextMessageAsync(
                chatId: -1002394470457,
                text: messages.ToString(),
                cancellationToken: cts.Token
            );


            // Stop receiving updates
            cts.Cancel();
        }

        // Handle incoming updates
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;

            var message = update.Message;
            if (message?.Text != null)
            {
                Console.WriteLine($"Received a message from {message.Chat.Username}: {message.Text}");

                await botClient.SendTextMessageAsync(
                    chatId: -1002394470457,
                    text: $"You said: {message.Text}",
                    cancellationToken: cancellationToken
                );
            }
        }

        public StringBuilder CheckDataStatus(Dictionary<string, object> data, string fwName)
        {
            StringBuilder tasks = new StringBuilder();

            // Corrected logic to alert on high resource usage (80% or above)
            if (Convert.ToInt32(data["cpu"]) >= 80)
            {
                string message = $"🚨 Alert: Firewall {fwName} CPU usage is high! Current CPU: {data["cpu"]}% 🚨\n\n";
                tasks.Append(message);
            }

            if (Convert.ToInt32(data["rx_error_total"]) >= 1)
            {
                string message = $"🚨 Alert: Firewall {fwName} has Received Package Errors! Total Errors: {data["rx_error_total"]} 🚨\n\n";
                tasks.Append(message);
            }

            if (Convert.ToInt32(data["tx_error_total"]) >= 1)
            {
                string message = $"🚨 Alert: Firewall {fwName} has Transmit Package Errors! Total Errors: {data["tx_error_total"]} 🚨\n\n";
                tasks.Append(message);
            }

            if (data["raid_state"]?.ToString() == "FAILED")
            {
                string message = $"🚨 Alert: Firewall {fwName} RAID state is FAILED! 🚨\n\n";
                tasks.Append(message);
            }

            if (data["licence_status"]?.ToString() == "none")
            {
                string message = $"🚨 Alert: Firewall {fwName} has no licenses! 🚨\n\n";
                tasks.Append(message);
            }

            if (data["hotfix"]?.ToString() == "No Hotfix Found")
            {
                string message = $"🚨 Alert: Firewall {fwName} has no Hotfixes! 🚨\n\n";
                tasks.Append(message);
            }

            if (Convert.ToInt32(data["fwtmp"]) >= 80)
            {
                string message = $"🚨 Alert: Firewall {fwName} disk space /fwtmp is almost full at {data["fwtmp"]}% 🚨\n\n";
                tasks.Append(message);
            }

            if (Convert.ToInt32(data["varloglog"]) >= 80)
            {
                string message = $"🚨 Alert: Firewall {fwName} disk space /var/log is almost full at {data["varloglog"]}% 🚨\n\n";
                tasks.Append(message);
            }

            return tasks;
        }

        // Handle errors
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
