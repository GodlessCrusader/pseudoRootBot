// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.IO;

List<Session> activeSessions = new List<Session>();


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

List<long> chats = new List<long>();

var receiverOptions = new ReceiverOptions { // receives all the shit you send
    AllowedUpdates = { }
};

botClient.StartReceiving(                                                                                   // Client initialize 
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token
);


var me = await botClient.GetMeAsync();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
Console.ReadLine();





async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)      //On update
{
    if (update.Type != UpdateType.Message)
        return;
    var messageText = update.Message.Text;
    
    Session currentSession;
    if(activeSessions.Exists(x => x.ChatId == update.Message.Chat.Id))
    {
        currentSession = activeSessions.Find(x => x.ChatId == update.Message.Chat.Id);
    }
    else
    {
        if(messageText == @"/start")
        { 
            currentSession = Session.Start(botClient, update.Message.Chat.Id);
            activeSessions.Add(currentSession); 
        }
        else
        {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, "Please clear message history and use /start for proper registration");
            return;
        }   
    }
    var fileName = currentSession.FileSystemDoc.FileId;

    if(update.Message.Type == MessageType.Document)                                  //Drag'n'drop
    {
        var mesId = update.Message.MessageId;
        var documentName = update.Message.Document.FileName;
        CreateCommand.Handle(currentSession.Pwd, fileName, documentName, mesId);
        return;
    }
    
    List<string> cmdLn = messageText!.Split(" ").ToList<string>();
    
    foreach(string s in cmdLn)
    {
        Console.WriteLine(s);
    }
    
    if(commandList.Exists(x => x.Name == cmdLn[0]))                                                                      // Executes Command 
    {
        try
        {
            string? mess = commandList.Find(x => x.Name == cmdLn[0])!.Handle(messageText, currentSession.Pwd, fileName, currentSession.ChatId);
            
            if(mess!=null)
            {
                await botClient.SendTextMessageAsync(
                chatId: currentSession.ChatId,
                text: mess ,
                cancellationToken: cancellationToken);
            }
        }
        
        catch(Exception ex)
        {
        await botClient.SendTextMessageAsync(
        chatId: currentSession.ChatId,
        text: ex.ToString() ,
        cancellationToken: cancellationToken); 
        }
    }

    Message sentMessage = await botClient.SendTextMessageAsync(                                                             // Echo the present working directory
        chatId: currentSession.ChatId,
        text: currentSession.Pwd.GetString() ,
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

// void ShowChats(){
//     int i = 1;
//     foreach (long id in chats) {
//         Console.WriteLine($"id {i++} : {id}");
//     }
//     Console.WriteLine();
// }