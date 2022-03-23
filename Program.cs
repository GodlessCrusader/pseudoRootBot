// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.IO;
// using static Command;
// using static FilePath;

FilePath pwd = new FilePath();

var botClient = new TelegramBotClient("5106073089:AAFUWHZLl7BN0qedxn41BRyVFPoIMjz9KB4");

var cts  = new CancellationTokenSource();

List<Command> commandList = new List<Command>() 
{
    new CdCommand(botClient, cts.Token),
    new MkdirCommand(botClient, cts.Token),
    new DeleteCommand(botClient, cts.Token),
    new TreeCommand(botClient, cts.Token),
    new CreateCommand(botClient, cts.Token),
    new LsCommand(botClient, cts.Token)
};

foreach(Command c in commandList)
{
    Console.WriteLine($"Name : {c.Name}");
}

List<long> chats = new List<long>();



var receiverOptions = new ReceiverOptions { // receives all the shit you send
    AllowedUpdates = { }
};

//  using (StreamReader sr = System.IO.File.OpenText("testFS.Json"))
//         {
//             fileTree = sr.ReadToEnd();
//         }


botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token
);


var me = await botClient.GetMeAsync();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
Console.ReadLine();





async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Type != UpdateType.Message)
        return;
    if(update.Message.Type == MessageType.Document)
    {
        var mesId = update.Message.MessageId;
        var documentName = update.Message.Document.FileName;
        CreateCommand.Handle(pwd, "testFS.json", documentName, mesId);
        return;
    }

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;
    Console.WriteLine(messageText);
        if (!chats.Exists(y => y == chatId))
    {
        chats.Add(chatId);
    }

    List<string> cmdLn = messageText!.Split(" ").ToList<string>();
    Console.WriteLine("cmdLn:");
    Console.WriteLine($" 0 element: {cmdLn[0]}");
    foreach(string s in cmdLn)
    {
        Console.WriteLine(s);
    }
    if(commandList.Exists(x => x.Name == cmdLn[0]))
    {
        try
        {
            string? mess = commandList.Find(x => x.Name == cmdLn[0])!.Handle(messageText, pwd, "testFS.json", chatId);
            if(mess!=null)
            {
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: mess ,
                cancellationToken: cancellationToken);
            }
        }
        catch(Exception ex)
        {
        await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: ex.ToString() ,
        cancellationToken: cancellationToken);
  
        }
    }
      // returns Command with corresponding name


    ShowChats();

    // Echo 
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: pwd.GetString() ,
        cancellationToken: cancellationToken);
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

void ShowChats(){
    int i = 1;
    foreach (long id in chats) {
        Console.WriteLine($"id {i++} : {id}");
    }
    Console.WriteLine();
}