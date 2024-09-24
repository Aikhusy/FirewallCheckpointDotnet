using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Firewall
{
    public interface INotif
    {
        // Method that initializes and runs the bot
        Task RunBot(StringBuilder messages);

        // Method to check firewall data and return the message
        StringBuilder CheckDataStatus(Dictionary<string, object> data, string fwName);

        // Method to handle incoming updates
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);

        // Method to handle errors
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
    }
}
