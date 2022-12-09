using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    class Bot
    {
        static void Main()
        {
            while (true)
            {
                try
                {
                    MainLoop().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MAIN LOOP EXIT ERROR - " + ex);
                    Thread.Sleep(30000);
                }
            }
        }
        static async Task MainLoop()
        {
            // Read Configuration
            var telegramKey = ConfigurationManager.AppSettings["TelegramKey"];

            // Start Bot
            var bot = new TelegramBotClient(telegramKey);
            var me = await bot.GetMeAsync();
            Console.WriteLine(me.Username + " started at " + DateTime.Now);

            var offset = 0;
            while (true)
            {
                var updates = new Update[0];
                try
                {
                    if (DateTime.Now.Hour == 12 && DateTime.Now.Minute == 10 && (DateTime.Now.Second >= 0 && DateTime.Now.Second <= 2))
                    {
                        List<CourseInfo> parsedCourses = new List<CourseInfo>();
                        List<CourseInfo> newCourses = new List<CourseInfo>();
                        parsedCourses.AddRange(ParserManager.ParseITea());
                        parsedCourses.AddRange(ParserManager.ParseGoIT());
                        parsedCourses.AddRange(ParserManager.ParseBeetRootAcademy());
                        parsedCourses.AddRange(ParserManager.ParseMateAcademy());
                        foreach (var item in parsedCourses)
                        {
                            if (!SQLManager.IsCourseInDB(item.Name, item.Link).Result) newCourses.Add(item);
                        }
                        SQLManager.ClearAllWritesInTable("[dbo].[Courses]");
                        SQLManager.AddCourses(parsedCourses);
                        newCourseNotification(newCourses);
                        
                    }
                    updates = await bot.GetUpdatesAsync(offset);
                }
                catch (TaskCanceledException)
                {
                    // Don't care
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR WHILE GETTIGN UPDATES - " + ex);
                }
                foreach (var update in updates)
                {
                    offset = update.Id + 1;
                    ProcessUpdate(bot, update, me);
                }

                await Task.Delay(1000);
            }
        }

        static async void ProcessUpdate(TelegramBotClient bot, Update update, User me)
        {
            // Process Request
            try
            {
                var text = update.Message.Text;
                var replyText = string.Empty;
                if (text != null && (text.StartsWith("/", StringComparison.Ordinal) || text.StartsWith("!", StringComparison.Ordinal)))
                {
                    // Log to console
                    Console.WriteLine(update.Message.Chat.Id + " < " + update.Message.From.Username + " - " + text);
                    // Log to DB
                    if (!SQLManager.IsUserInDB(update.Message.Chat.Id).Result)
                    {
                        SQLManager.AddNewUser(update.Message.Chat.Id);
                    }
                    // Allow ! or /
                    if (text.StartsWith("!", StringComparison.Ordinal))
                    {
                        text = "/" + text.Substring(1);
                    }

                    // Strip @BotName
                    text = text.Replace("@" + me.Username, "");

                    // Parse
                    string command;
                    string body;
                    if (text.StartsWith("/s/", StringComparison.Ordinal))
                    {
                        command = "/s"; // special case for sed
                        body = text.Substring(2);
                    }
                    else
                    {
                        command = text.Split(' ')[0];
                        body = text.Replace(command, "").Trim();
                    }
                    var stringBuilder = new StringBuilder();
                    ParseMode? parseMode = null;
                    switch (command.ToLowerInvariant())
                    {
                        case "/start":
                            replyText = "Привіт! Я QWERTY бот, створений для того щоб допомогти тобі" +
                                " знайти курси по програмуванню! Напиши /help, щоб дінатися про усі команди";
                            break;

                        case "/help":
                            replyText = "Ось список усіх команд :\n" +
                                "/add <Тег> - додати або замінити тег (мова програмування) для пошуку курсів\n" +
                                "/clear - видалення всіх тегів\n" +
                                "/check - перевірити усі наявні курси за тегом\n";
                            break;

                        case "/check":
                            if (SQLManager.GetTagById(update.Message.Chat.Id).Result != "NULL")
                            {
                                var courses = SQLManager.FindAllCoursesByTag(SQLManager.GetTagById(update.Message.Chat.Id).Result).Result;
                                replyText = $"За вашим тегом '{SQLManager.GetTagById(update.Message.Chat.Id).Result}' зайдено {courses.Count} курсів\n";
                                foreach (var item in courses)
                                {
                                    replyText += $"[ТИК]({item.Link}) {item.Name.Trim().ToUpper()}\n";
                                }
                                parseMode = ParseMode.Markdown;
                            }
                            else
                            {
                                replyText += "Ваш тег пустий!";
                            }
                            break;

                        case "/add":
                            if (body == string.Empty)
                            {
                                replyText = "Використання: /add <Тег>";
                            }
                            else if (body.ToLower() == SQLManager.GetTagById(update.Message.Chat.Id).Result)
                            {
                                replyText = "Такий тег вже є! Нашо два рази?";
                                break;
                            }
                            else if (body.Length >= 40)
                            {
                                replyText = "Тег надто довгий, спробуй інше!";
                                break;
                            }
                            else
                            {
                                SQLManager.UpdateTag(update.Message.Chat.Id, body.ToLower());
                                replyText = $"Тег \"{body}\" додано!";
                            }
                            break;

                        case "/clear":
                            if (SQLManager.GetTagById(update.Message.Chat.Id).Result != "NULL")
                            {
                                SQLManager.CLearTagList(update.Message.Chat.Id);
                                replyText = $"Теги \"{SQLManager.GetTagById(update.Message.Chat.Id).Result}\" видалено!";
                            }
                            else
                            {
                                replyText = $"Нічого видаляти, там і так пусто(empty)!";
                            }
                            break;

                        default:
                            replyText = "Напиши /help, щоб дізнатися про усі команди!";
                            break;

                    }

                    // Output
                    replyText += stringBuilder.ToString();
                    if (!string.IsNullOrEmpty(replyText))
                    {
                        Console.WriteLine(update.Message.Chat.Id + " > " + replyText);
                        await bot.SendTextMessageAsync(update.Message.Chat.Id, replyText, parseMode, disableWebPagePreview: true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - " + ex);
            }
        }
        static async void newCourseNotification(List<CourseInfo> newCourses)
        {
            var telegramKey = ConfigurationManager.AppSettings["TelegramKey"];
            var bot = new TelegramBotClient(telegramKey);
            var users = SQLManager.GetAllUsers().Result;
            foreach (var user in users)
            {
                foreach (var item in newCourses)
                {
                    if (item.Name.Contains(user.Tags.ToLower()))
                    {
                        await bot.SendTextMessageAsync(user.Id, $"Знайдено новий курс: {item.Name.ToUpper()}\nПеревір /check");
                    }
                }
            }
        }
    }
}