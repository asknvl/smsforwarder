using smsforwarder.server;
using smsforwarder.users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace smsforwarder.bot
{
    internal class bot_forwarder
    {
        #region const
#if DEBUG
        string Token = "5873996687:AAE-Q7GHWqtkBEFUyIUYFV9nsz3z4YjSxes";
#else
        string Token = "";
#endif
        #endregion

        #region vars

#if DEBUG
        IServerApi serverApi = new ServerApi("");
#else
        IServerApi serverApi = new ServerApi("");
#endif

        TelegramBotClient bot;
        CancellationTokenSource cts;
        UserManager userManager;
        System.Timers.Timer smsReadTimer;
        #endregion

        #region helpers
        async Task send(string msg, Update upd, CancellationToken ct)
        {
            try
            {
                Message sentMessage = await bot.SendTextMessageAsync(
                               chatId: upd.Message.Chat.Id,
                               text: msg,
                               cancellationToken: ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task sendAll(string msg, CancellationToken ct)
        {
            foreach (var user in userManager.users)
            {
                if (user.NeedEmergencyNotify)
                {
                    try
                    {
                        await bot.SendTextMessageAsync(
                             chatId: user.Id,
                             text: msg,
                             cancellationToken: ct);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        #endregion

        #region private
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            switch (update.Type)
            {
                case UpdateType.MyChatMember:
                    if (update.MyChatMember != null)
                    {
                        long id = update.MyChatMember.Chat.Id;
                        if (update.MyChatMember.NewChatMember.Status == ChatMemberStatus.Administrator)
                            userManager.Add(id, "channel");
                    }
                    break;

                case UpdateType.Message:
                    if (update.Message != null)
                    {
                        long id = update.Message.Chat.Id;
                        string msg = update.Message.Text;
                        string name = $"{update.Message.Chat.FirstName} {update.Message.Chat.LastName}";

                        if (msg != null)
                        {
                            if (!userManager.Check(id) && !msg.Equals("4444"))
                            {
                                await send("Нет доступа", update, cancellationToken);
                                return;
                            }

                            switch (msg)
                            {
                                case "4444":
                                    userManager.Add(id, name);
                                    break;
                            }

                            if (msg.Contains("notify"))
                            {
                                msg = msg.Replace("notify", "");
                                await sendAll(msg, cts.Token);
                            }
                        }
                    }
                    break;
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)

        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private void SmsReadTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {

        }

        #endregion

        #region public
        public void Start()
        {
            userManager = new UserManager();
            userManager.Init();

            bot = new TelegramBotClient(Token);
            cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.ChannelPost, UpdateType.MyChatMember }
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            smsReadTimer = new System.Timers.Timer();
            smsReadTimer.Interval = 1000;
            smsReadTimer.AutoReset = true;
            smsReadTimer.Elapsed += SmsReadTimer_Elapsed;
            smsReadTimer.Start();
        }



        #endregion
    }
}
