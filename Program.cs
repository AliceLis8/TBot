using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

using ApiAiSDK;
using ApiAiSDK.Model;

namespace TBot2209
{
    class Program
    {
        static TelegramBotClient Bot;

        static ApiAi apiAi;


        static void Main(string[] args)
        {
            
            //d9579975abae4cc3a22686d10a80d6ff 

            Bot = new TelegramBotClient("871461430:AAG3CPuRQl_Lk1AXlpPY8qePy9ufHF_wTHw");

            AIConfiguration config = new AIConfiguration("d9579975abae4cc3a22686d10a80d6ff", SupportedLanguage.English);

            apiAi = new ApiAi(config);

            var me = Bot.GetMeAsync().Result;

            Console.WriteLine(me.FirstName);

            Bot.OnMessage += BotOnMessageReceived;

            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;

            Bot.StartReceiving();

            Console.ReadLine();

            Bot.StopReceiving();
                       
            
            }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} pish battom {buttonText}");

            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"You push the batton {buttonText}");

            try
            {
                if (buttonText == "pic")
                {
                    await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, " https://www.facebook.com/ArtofSilverFox/photos/a.649754801835346/738289212981904/?type=3&theater ");

                }

                else if (buttonText == "vidio")
                {
                    await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.youtube.com/watch?v=nEf7HyZPD7E ");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }


        private static async void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message == null || message.Type != MessageType.Text)
                return;

            string name = $"{message.From.FirstName} {message.From.LastName}";

            Console.WriteLine($"{name} send message: {message.Text}");

            switch (message.Text)
            {
                case "/start":
                    string text =
                        @"List of commands:
                         /start - launch bot
                         /inline - Choose social network
                         /keyboard - keyboard output";
                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                case "/inline":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("VK", "https://vk.com/jetfest2019"),
                            InlineKeyboardButton.WithUrl("FB", "https://www.facebook.com/"),
                            InlineKeyboardButton.WithUrl("INST", "https://www.instagram.com/")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("pic"),
                            InlineKeyboardButton.WithCallbackData("vidio")
                        }

                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Choose menu item", replyMarkup: inlineKeyboard);
                    break;

                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Hi! "),
                            new KeyboardButton("How are you?")
                        },

                        new[]
                        {
                            new KeyboardButton("Contact") {RequestContact = true },
                            new KeyboardButton("Location") {RequestLocation = true }
                        }
                    });

                    await Bot.SendTextMessageAsync(message.Chat.Id, "one smart person said nothing because times were hard and people were not reliable", replyMarkup: replyKeyboard);
                    break;

                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                        answer = "Sorry, I don't understand you";

                    await Bot.SendTextMessageAsync(message.From.Id, answer);

                    break;

            }



        }
    }
}
