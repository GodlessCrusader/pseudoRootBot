// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.IO;

List<Session> activeSessions = new List<Session>();

var timeoutCheckThread = new Thread(() => TimeoutCheck(activeSessions));

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
// botClient.UnpinAllChatMessages(480568360);
var me = await botClient.GetMeAsync();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
Console.ReadLine();


async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)      //On update
{
    
    if(activeSessions.Exists(x => x.ChatId == update.Message.Chat.Id))
    {
        Console.WriteLine("Entered Reset check");
        activeSessions.Find(x => x.ChatId == update.Message.Chat.Id).ResetTimeout(1);
    }
    if (update.Type != UpdateType.Message)
        return;
    
    Session currentSession;
    
    if(activeSessions.Exists(x => x.ChatId == update.Message.Chat.Id))
    {
        currentSession = activeSessions.Find(x => x.ChatId == update.Message.Chat.Id); 
    }
    else
    {
        try
        {
            currentSession = Session.Start(botClient, update.Message.Chat.Id);
            Console.WriteLine("Started session");
            activeSessions.Add(currentSession);
            Console.WriteLine("added to active sessions");
        }
        catch(Exception e)
        {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, e.ToString());
            return;
        }
    }
    Console.WriteLine($"Current session rootdir name:{currentSession.RootDir == null}");
    

    var fileName = $"{currentSession.ChatId.ToString()}.json";

    List<MessageType> allowedTypes = new List<MessageType>()
    {
        MessageType.Photo,
        MessageType.Video,
        MessageType.Audio,
        MessageType.Contact,
        MessageType.Document,
        MessageType.Location,
        MessageType.Voice,
        MessageType.VideoNote,
        MessageType.SuccessfulPayment
    };
    if(allowedTypes.Exists(x => x == update.Message.Type))                                  //Drag'n'drop
    {
        var mesId = update.Message.MessageId;
        string? documentName = null;
        if(update.Message.Document != null)
            documentName = update.Message.Document.FileName;
        await Task.Run(() => 
        CreateCommand.Handle(currentSession.Pwd, fileName, documentName, mesId));
        await botClient.SendTextMessageAsync(                                                             // Echo the present working directory
        chatId: currentSession.ChatId,
        text: currentSession.Pwd.GetString() ,
        cancellationToken: cancellationToken);
        return;
    }
    if(update.Message.Text == null)
        return;
    
    List<string> cmdLn = update.Message.Text!.Split(" ").ToList<string>();
    

    if(cmdLn[0] == "close")
    {
        currentSession.Close();
        activeSessions.Remove(currentSession);
        botClient.SendTextMessageAsync(update.Message.Chat.Id,"Session is over. Write anything to start using pseudoroot");
        return;    
    }


    if(commandList.Exists(x => x.Name == cmdLn[0]))                                                                      // Executes Command 
    {
        try
        {
            // string? mess = 
            commandList.Find(x => x.Name == cmdLn[0])!.Handle(update.Message.Text, currentSession);
            
            // if(mess!=null)
            // {
            //     await botClient.SendTextMessageAsync(
            //     chatId: currentSession.ChatId,
            //     text: mess ,
            //     cancellationToken: cancellationToken);
            // }
        }
        
        catch(Exception ex)
        {
        await botClient.SendTextMessageAsync(
        chatId: currentSession.ChatId,
        text: ex.ToString() ,
        cancellationToken: cancellationToken); 
        }
    }
    if(!timeoutCheckThread.IsAlive) 
    {
        timeoutCheckThread = new Thread(() => TimeoutCheck(activeSessions));
        timeoutCheckThread.Start();
    }

    await botClient.SendTextMessageAsync(                                                             // Echo the present working directory
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

void TimeoutCheck(List<Session> aS) //Thread secure rewrite
{
    while(aS.Count > 0)
    {
        // Console.WriteLine(DateTime.Now - aS[0].ClosureTime);
        if(aS.Exists(x => x.ClosureTime < DateTime.Now))
        {
            

            foreach(Session s in aS.FindAll(x => x.ClosureTime < DateTime.Now))
            {
                Console.WriteLine("Closed");
                s.Close();
            }
            aS.RemoveAll(x => x.ClosureTime < DateTime.Now);
        }
    }
}
// void ShowChats(){
//     int i = 1;
//     foreach (long id in chats) {
//         Console.WriteLine($"id {i++} : {id}");
//     }
//     Console.WriteLine();
// }