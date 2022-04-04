using Telegram.Bot;
using Telegram.Bot.Types;

class Session 
{
    public string FileId;
    private DateTime closureTime;
    public DateTime ClosureTime
    {
        get{
            return closureTime;
        }
    }
    private long chatId;
    public long ChatId
    {
        get{
            return chatId;
        }
    }
    private ITelegramBotClient botClient;
    private Telegram.Bot.Types.File? fileSystemDoc;
    public Telegram.Bot.Types.File? FileSystemDoc
    {
        get{
            return fileSystemDoc;
        }
    }
    private FilePath pwd = new FilePath();
    public FilePath Pwd
    {
        get{
            return pwd;
        }
    }

    bool startFlag = false;
    public bool StartFlag
    {
        get{
            return startFlag;
        }
    }

    private Session(ITelegramBotClient botClient, long chatId)
    {
        startFlag = true;
        this.chatId = chatId;
        this.botClient = botClient;
        closureTime = DateTime.Now.AddMinutes(2);
        try
        {
            fileSystemDoc = botClient.GetFileAsync(botClient.GetChatAsync(chatId).Result.PinnedMessage.Document.FileId).Result;    
        }
        catch
        {
            fileSystemDoc = null;
        }
    }

    public static Session Start(ITelegramBotClient botClient, long chatId)
    {
        var ses = new Session(botClient, chatId);
        ses.ResetTimeout(5);
        if(ses.fileSystemDoc == null)
            {
                ses.RegisterNewUser();
            }
        else
            ses.Open();
        return ses;
    }
    
    private async void Open()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        using(FileStream transfer = System.IO.File.Open($"{chatId.ToString()}.json", FileMode.Create))
        {
            await botClient.DownloadFileAsync(fileSystemDoc.FilePath,transfer,cts.Token);
        }
        
    }

    public void Close()
    {
        startFlag = false;
            using(FileStream transfer = System.IO.File.Open($"{chatId.ToString()}.json", FileMode.Open))
            {
            Message m = botClient.EditMessageMediaAsync(
                chatId: chatId,
                messageId: botClient.GetChatAsync(chatId).Result.PinnedMessage.MessageId,
                media: new InputMediaDocument(new InputMedia(transfer, $"{chatId.ToString()}.json"))).Result;
            }
            System.IO.File.Delete($"{chatId.ToString()}.json");
            Message s = botClient.SendTextMessageAsync(chatId,"Session is over. Write anything to start using pseudoroot").Result;

    }

    private void RegisterNewUser()
    {
        using(StreamWriter fs = new StreamWriter($"{chatId.ToString()}.json"))
        {
            fs.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new Directory("rom",null)));  
        }
        Message m = new Message();
        using(FileStream transfer = System.IO.File.Open($"{chatId.ToString()}.json", FileMode.OpenOrCreate))
        {
            m = botClient.SendDocumentAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(transfer),null,$"{chatId.ToString()}.json").Result;
            botClient.PinChatMessageAsync(chatId,m.MessageId);
            fileSystemDoc = botClient.GetFileAsync(m.Document.FileId).Result;
        }
    }

    public void ResetTimeout(int time)
    {
        closureTime = DateTime.Now.AddMinutes(5);
    }

   
    
}