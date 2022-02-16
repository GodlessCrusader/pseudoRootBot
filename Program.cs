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

string fileTree = "";
Directory rootDirectory = new Directory("rom",null);
FilePath pwd = new FilePath();
List<Command> commandList = new List<Command>();
commandList.Add(new CdCommand());
foreach(Command c in commandList)
{
    Console.WriteLine($"Name : {c.Name}");
}
List<long> chats = new List<long>();
var botClient = new TelegramBotClient("5106073089:AAFUWHZLl7BN0qedxn41BRyVFPoIMjz9KB4");
var cts  = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions { // receives all the shit you send
    AllowedUpdates = { }
};

//  using (StreamReader sr = System.IO.File.OpenText("testFS.Json"))
//         {
//             fileTree = sr.ReadToEnd();
//         }

 using (StreamWriter sw = System.IO.File.CreateText("testFS.Json"))
        {
            sw.Write(fileTree);
            
        }

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
    // Only process text messages
    if (update.Message!.Type != MessageType.Text) 
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;
    Console.WriteLine(messageText);
        if (!chats.Exists(y => y == chatId))
    {
        chats.Add(chatId);
    }

    List<string> cmdLn = messageText.Split(" ").ToList<string>();
    Console.WriteLine("cmdLn:");
    Console.WriteLine($" 0 element: {cmdLn[0]}");
    foreach(string s in cmdLn)
    {
        Console.WriteLine(s);
    }
    if(commandList.Exists(x => x.Name == cmdLn[0]))
    {
        // Console.WriteLine("entered");
        commandList.Find(x => x.Name == cmdLn[0]).Handle(messageText, pwd);
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