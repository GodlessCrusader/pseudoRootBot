using Telegram.Bot;
using Telegram.Bot.Types;

class Session 
{
    private int timer = 5000;
    private long chatId;
    public long ChatId
    {
        get{
            return chatId;
        }
    }
    private ITelegramBotClient botClient;
    private Telegram.Bot.Types.File fileSystemDoc;
    public Telegram.Bot.Types.File FileSystemDoc
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

    bool activityFlag = false;
    public bool ActivityFlag
    {
        get{
            return activityFlag;
        }
    }

    private bool status = false;

    private Session(ITelegramBotClient botClient, long chatId)
    {
        status = true;
        this.chatId = chatId;
        this.botClient = botClient;
        fileSystemDoc = botClient.GetFileAsync($"{chatId.ToString()}.json").Result;
    }

    public static Session Start(ITelegramBotClient botClient, long chatId)
    {
        var ses = new Session(botClient, chatId);

        return ses;
    }
    
    private void Open()
    {
        using(FileStream transfer = System.IO.File.Create($"{chatId.ToString()}.json"))
        {
            botClient.DownloadFileAsync(fileSystemDoc.FilePath, transfer);
        }
        
    }

    public void Close()
    {
        status = false;
        using(FileStream transfer = System.IO.File.Open($"{chatId.ToString()}.json", FileMode.Open))
        {
            botClient.EditMessageMediaAsync(
                chatId: chatId,
                messageId: 2,
                media: new InputMediaDocument(new InputMedia(transfer, $"{chatId.ToString()}.json")));
        }
    }

    private void RegisterNewUser()
    {
        startFlag = true;
        using(StreamWriter fs = new StreamWriter($"{chatId.ToString()}.json"))
        {
            fs.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new Directory("rom",null)));
            FileStream transfer = System.IO.File.Open($"{chatId.ToString()}.json", FileMode.OpenOrCreate);
            botClient.SendDocumentAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(transfer));
            botClient.SendTextMessageAsync(chatId,"Complete");
        }
    }

    public void ResetTimeout(int time)
    {
        
    }

   
    
}