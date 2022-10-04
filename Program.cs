// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.IO;
using PseudoRoot;

List<Session> activeSessions = new List<Session>();

object timeoutCheckThreadLock = new object();

bool timeoutCheckFlag = false;

var botClient = new TelegramBotClient(args[0]); //Change

var cts  = new CancellationTokenSource();

var timeoutCheckThread = new Thread(() => TimeoutCheck(activeSessions));

var receiverOptions = new ReceiverOptions { // receives all you send
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
    Session currentSession;
    if(activeSessions.Exists(x => x.ChatId == update.Message!.Chat.Id))
    {
        Console.WriteLine("Entered Reset check");
        currentSession = activeSessions.Find(x => x.ChatId == update.Message!.Chat.Id)!;
        currentSession.ResetTimeout(3);
        if(currentSession.IsPerforming != null && update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            int IsPerformingCommandsCount = 0;
            if(currentSession.IsPerforming.CorrespondingCommands != null)
                IsPerformingCommandsCount = currentSession.IsPerforming.CorrespondingCommands.Count;
        
            Console.WriteLine($"Started performingArgs handling");
            currentSession.performingArgs.Add($"!{update.Message.Text}!"); 
            if(currentSession.IsPerforming.CorrespondingCommands != null)
            {
                if(currentSession.IsPerforming.CorrespondingCommands.Count>0)
                {
                    Console.WriteLine("Deguing corresponding command");
                    currentSession.IsPerforming.CorrespondingCommands.Dequeue().Handle(new List<string>(), currentSession);
                    return;
                }
    
            }
            

            if(currentSession.IsPerforming.CorrespondingCommands == null || currentSession.IsPerforming.CorrespondingCommands.Count == 0)
            {
                string cmd;
                if((currentSession.IsPerforming.CorrespondingCommands != null && currentSession.IsPerforming.CorrespondingCommands.Count == 0 && currentSession.performingArgs.Count == IsPerformingCommandsCount))
                {
                    currentSession.performingArgs.Add($"!{update.Message.Text}!");
                }
                
                if(currentSession.performingArgs.Count == 0)
                {
                    cmd = $"{currentSession.IsPerforming.Name} {update.Message.Text}";    
                }
                else
                {
                    cmd = String.Join("", currentSession.performingArgs);
                    currentSession.performingArgs.Clear();
                }
                Console.WriteLine($"Performing args:{cmd}");
                currentSession.IsPerforming.Handle(AliesTranslation.ParseCommandLine($"{currentSession.IsPerforming.Name} {cmd}"), currentSession);
                currentSession.IsPerforming = null;
                return;
            }
        }
    }
    else
    {
        try
        {
            currentSession = Session.Start(botClient, update.Message!.Chat.Id);
            Console.WriteLine("Started session");
            activeSessions.Add(currentSession);
            Console.WriteLine("added to active sessions");
            currentSession.ChangeKeyboard(null);

        }
        catch(Exception e)
        {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, e.ToString());
            return;
        }
    }
    Console.WriteLine($"Current session rootdir name:{currentSession.RootDir.Name}");
    

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
        var mesId = update.Message.MessageId.ToString();
        string? documentName = null;
        if(update.Message.Document != null)
            documentName = update.Message.Document.FileName;
        if(currentSession.CommandList.Exists(x => x is CreateCommand))
            currentSession.CommandList.Find(x => x is CreateCommand)!.Handle(new List<string?>(){documentName, mesId} ,currentSession);
        return;
    }
    if(update.Message.Text == null)
        return;
    string? cmdLn = update.Message.Text;
    if(currentSession.PreferedMode == CommandMode.InternalKeyboard)
    {
        cmdLn = AliesTranslation.TranslateKeyboardCommand(cmdLn, currentSession, currentSession.CommandList);
        if(cmdLn == null)
        {
            Console.WriteLine("isn't right");
            return;
        }
    }
    List<string> cmds = AliesTranslation.ParseCommandLine(cmdLn);   
    cmdLn = String.Join(' ',cmds);
    Console.WriteLine($"isn't right{cmdLn}");

    if(cmds[0] == "close")
    {
        timeoutCheckFlag = true;
        lock(timeoutCheckThreadLock)
        {
            currentSession.Close();
            activeSessions.Remove(currentSession);
        }
        return;    
    }

    if( currentSession.CommandList.Exists(x => x.Name == cmds[0]))                                    // Executes Command 
    {
        Console.WriteLine($"Try block, command:{cmds[0]}, cmdLn: {cmdLn}");
        currentSession.CommandList.Find(x => x.Name == cmds[0])!.Handle(cmds, currentSession);
    }
    if(!timeoutCheckThread.IsAlive && activeSessions.Count>0) 
    {
        timeoutCheckThread = new Thread(() => TimeoutCheck(activeSessions));
        timeoutCheckThread.Start();
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

void TimeoutCheck(List<Session> aS) 
{
   
    
    while(true)
    {   
        try
        {
            lock(timeoutCheckThreadLock)
            {
                if(aS.Exists(x => x.ClosureTime < DateTime.Now))
                {
                    foreach(Session s in aS.FindAll(x => x.ClosureTime < DateTime.Now))
                    {
                        Console.WriteLine("Closed");
                        s.Close();
                    }
                    aS.RemoveAll(x => x.ClosureTime < DateTime.Now);
                }
                if(!(aS.Count>0) || timeoutCheckFlag)
                {
                    timeoutCheckFlag = false;
                    return;
                }
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }   

}

