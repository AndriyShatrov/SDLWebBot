using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("200554092:AAF6KSlbqp3yXsGBtxaC3r0bNRlEa6kVzb4");

        private static Dictionary<long, string> users = new Dictionary<long, string>();

        static void Main(string[] args)
        {

            Timer timer = new Timer(60000);
            timer.Elapsed += TimerOnElapsed;
            timer.Enabled = true;

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            //Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnReceiveError += BotOnReceiveError;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Console.WriteLine("Translation finished.");
            var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("UnPublish"),
                        new InlineKeyboardButton("Share"),
                    }
                });

            foreach (var user in users)
            {
                await Bot.SendTextMessageAsync(user.Key, "Successfull published!",
                    replyMarkup: keyboard);
            }
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.StartsWith("/subscribe")) // send inline keyboard
            {
                users.Add(message.Chat.Id, message.Chat.FirstName);

                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                await Bot.SendTextMessageAsync(message.Chat.Id, "Successfully subscribed");
            }
            else
            {
                var usage = @"Usage:
/subscribe   - send inline keyboard
/unsubscribe - send custom keyboard
/help    - send a photo
/start  - request location or contact
";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
        }
    }
}
