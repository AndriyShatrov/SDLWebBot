﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Tridion.ContentManager.CoreService.Client;

namespace SDLWebBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("200554092:AAHLRLTXqNyIhDvs9zO18F1nU4gJHMysN2o");
        private static Dictionary<long, string> users = new Dictionary<long, string>();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            //Bot.OnInlineQuery += BotOnInlineQueryReceived;
            //Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            var me = Bot.GetMeAsync().Result;

            Bot.StartReceiving();

        }
        //private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        //{
        //    Console.WriteLine($"Chosen: {inlineQueryEventArgs}");
        //}
        //private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        //{
        //    Console.WriteLine($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        //}

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.StartsWith("/subscribe")) // send inline keyboard
            {
                string user;
                if (!users.TryGetValue(message.Chat.Id, out user))
                    users.Add(message.Chat.Id, message.Chat.FirstName);
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Bot.SendTextMessageAsync(message.Chat.Id, "Successfully subscribed");
            }
            else if (message.Text.StartsWith("/unsubscribe")) // send inline keyboard
            {
                users.Remove(message.Chat.Id);
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Bot.SendTextMessageAsync(message.Chat.Id, "Successfully unsubscribed");
            }
            else if (message.Text.StartsWith("/me"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, $"Hello, {message.Chat.FirstName} {message.Chat.LastName}");
                await Bot.SendPhotoAsync(message.Chat.Id, "https://pbs.twimg.com/profile_images/798921508794101761/VVkwVVxm_400x400.jpg");
            }
            else if (message.Text.StartsWith("/about"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "https://youtu.be/TycIBC5pPv0");
            }
            else if (message.Text.StartsWith("/install"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, $"Not so fast :)");
            }
            else if (message.Text.StartsWith("/balance"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, $"$400.00");
            }
            else
            {
                var usage = @"Usage:
/subscribe   - subscribe on CM events
/unsubscribe - unsubscribe on CM events
/install - install new CM environment
/balance - check your balance
/me - my information
/about - information about SDL
";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage,
                            replyMarkup: new ReplyKeyboardHide());
            }
        }

        public static async void PulicationFinished(string tcmUrl, string url)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("Open in CME"),
                        new InlineKeyboardButton("UnPublish", tcmUrl),
                    }
                });
            var message = $@"Successfully published!
{url}";
            foreach (var user in users)
            {
                await Bot.SendTextMessageAsync(user.Key, message, replyMarkup: keyboard);
            }
        }
        public static async void ComponentUpdated(string tcmUrl, string url)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("Publish"),
                    }
                });
            var message = $@"Component {url} ({tcmUrl}) updated and should be published again!";
            foreach (var user in users)
            {
                await Bot.SendTextMessageAsync(user.Key, message, replyMarkup: keyboard);
            }
        }
        public static async void ComponentNeedTranslation(string tcmUrl, string url, string childTCMUri, string childTitle )
        {
            var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("Go to translated component"),
                    }
                });
            var message = $@"Component {url} ({tcmUrl}) updated. Don't forget to update its translations:
{childTitle} ({childTCMUri})";
            foreach (var user in users)
            {
                await Bot.SendTextMessageAsync(user.Key, message, replyMarkup: keyboard);
            }
        }
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            string TCMUri = callbackQueryEventArgs.CallbackQuery.Data;
            if (TCMUri != null)
            {
                var client = new SessionAwareCoreServiceClient("wsHttp_201603");
                client.ClientCredentials.Windows.ClientCredential = new NetworkCredential("srv-cmtask", "srv_tridion_cm", "global");
                var instruction = new UnPublishInstructionData() { ResolveInstruction = new ResolveInstructionData() };
                var pt = client.UnPublish(new[] { TCMUri }, instruction, new[] { "purp1" }, null, new ReadOptions());
                await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Sucessfully unpublished {callbackQueryEventArgs.CallbackQuery.Data}", cacheTime: 0);
            }
            else
                await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Nothing", cacheTime: 0);
        }
    }

}
