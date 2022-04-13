// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
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
    new LsCommand(botClient, cts.Token),
    new GetCommand(botClient, cts.Token),
    new ChangeModeCommand(botClient, cts.Token)
};

Command? IsPerforming = null; 

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
        if(IsPerforming != null && update.Type == UpdateType.Message && update.Message.Text != null)
        {
            string cmd = $"{IsPerforming.Name} {update.Message.Text}";
            IsPerforming.Handle(cmd, activeSessions.Find(x => x.ChatId == update.Message.Chat.Id));
            IsPerforming = null;
        }
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
            currentSession.ChangeKeyboard(commandList);

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
        var mesId = update.Message.MessageId;
        string? documentName = null;
        if(update.Message.Document != null)
            documentName = update.Message.Document.FileName;
        CreateCommand.Handle(currentSession, documentName, mesId);
        await botClient.SendTextMessageAsync(                                                             // Echo the present working directory
        chatId: currentSession.ChatId,
        text: currentSession.Pwd.GetString(),
        replyMarkup: currentSession.Keyboard,
        cancellationToken: cancellationToken);
        return;
    }
    if(update.Message.Text == null)
        return;
    string cmdLn = update.Message.Text;
    if(currentSession.PreferedMode == CommandMode.InternalKeyboard)
    {
        cmdLn = TranslateKeyboardCommand(cmdLn, currentSession);
        if(cmdLn == null)
        {
            botClient.SendTextMessageAsync(currentSession.ChatId,"Error performing");
            return;
        }
    }
     List<string> cmds = ParseCommandLine(cmdLn);   
    // if(cmdLn.Contains('!'))
    cmdLn = String.Join(' ',cmds);

    if(cmds[0] == "close")
    {
        currentSession.Close();
        activeSessions.Remove(currentSession);
        return;    
    }

    // cmdLn = cmds[0];
    if(commandList.Exists(x => x.Name == cmds[0]))                                    // Executes Command 
    {
        try
        {
            Console.WriteLine($"Try block, command:{cmds[0]}, cmdLn: {cmdLn}");
            commandList.Find(x => x.Name == cmds[0])!.Handle(cmdLn, currentSession);
            currentSession.ChangeKeyboard(commandList);

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
        text: currentSession.Pwd.GetString(),
        replyMarkup: currentSession.Keyboard,
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

void TimeoutCheck(List<Session> aS) 
{
    while(aS.Count > 0)
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
    }
}

string TranslateKeyboardCommand(string cmdLn, Session session)
{
    string bashCmd = cmdLn;
    if(cmdLn.Contains("📁"))
    {
        bashCmd = $"cd {cmdLn.Replace("📁", "")}";
        Console.WriteLine($"bashcmd through translate metod:{bashCmd}");
    }

    if(cmdLn.Contains("📄"))
    {
        bashCmd = $"get !{cmdLn.Replace("📄", "")}!";
    }

    if(cmdLn.Contains("⤴️"))
    {
        bashCmd = $"cd {cmdLn.Replace("⤴️", "")}";
    }
    
    else if(commandList.Exists(x => x.Name == cmdLn) && commandList.Find(x => x.Name == cmdLn).RequiresArgument)
    {
        IsPerforming = commandList.Find(x => x.Name == cmdLn);
        botClient.SendTextMessageAsync(session.ChatId,IsPerforming.PerformingMessage,replyMarkup: new ReplyKeyboardRemove()).Wait();
        return null;
    }
    return bashCmd;
}

List<string> ParseCommandLine(string cmdLn)
{
    List<string>cmds = cmdLn!.Split(" ").ToList<string>();
    if(cmds.Count>1)   
    {   
        List<int> markupPostitions = new List<int>();
        for(int i = 0; i < cmds.Count; i++ )
        {
            var check = cmds[i].Contains("!");
            if(check)
            {   
                markupPostitions.Add(i);
            }
        }
        string arg = "";
        foreach(int num in markupPostitions)
        {
            arg+=cmds[num].Replace("!","");
        }
        cmds[1] = arg;
        Console.WriteLine(cmds[1]);
    }
    return cmds;
}