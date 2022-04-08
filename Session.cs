using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
class Session 
{
    private UserPreferences userPreferences;
    public CommandMode PreferedMode
    {
        set{
            userPreferences.PreferedMode = value;
        }

        get{
            return userPreferences.PreferedMode;
        }
    }
    public Directory RootDir{
        get{
            return userPreferences.RootDir;
        }
        set{
            userPreferences.RootDir = value;
        }
    }
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

    private ReplyKeyboardMarkup replyKeyboardMarkup = new(new []
    {
        new KeyboardButton[] { "One", "Two" },
        new KeyboardButton[] { "Three", "Four" },
        new KeyboardButton[] {new KeyboardButton("sas")}
    })
    {
        ResizeKeyboard = true
    };


    public ReplyKeyboardMarkup Keyboard
    {
        set{
            replyKeyboardMarkup = value;
        }

        get
        {
            return replyKeyboardMarkup;
        }

    }
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
        Console.WriteLine("New session object is created");
        ses.ResetTimeout(1);
        if(ses.fileSystemDoc == null)
            {
                ses.RegisterNewUser();
            }
        else
            {ses.Open();
            Console.WriteLine("session opened");}
        return ses;
    }
    
    private void Open()
    {
        Console.WriteLine($"fileSystemDoc: {fileSystemDoc.FilePath}");
        FileStream transfer = System.IO.File.Open($"{chatId.ToString()}.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);        
        botClient.DownloadFileAsync(fileSystemDoc.FilePath,transfer).Wait();
        transfer.Dispose();
        Console.WriteLine("done");
        using(StreamReader sr = new StreamReader($"{chatId.ToString()}.json"))
        {
            string jr = sr.ReadToEnd();
            Console.WriteLine($"Json Representation:{jr}");
            RootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(jr);
            Console.WriteLine($"RootDir.ToString() {RootDir == null}");
           
        }
        System.IO.File.Delete($"{chatId.ToString()}.json");
        
    }

    public void Close()
    {
        // System.IO.File.Create($"{chatId.ToString()}.json");
        StreamWriter sw = new StreamWriter(($"{chatId.ToString()}.json"));
        string jr = Newtonsoft.Json.JsonConvert.SerializeObject(RootDir);
        sw.Write(jr);
        sw.Dispose();
        FileStream fileStream = new FileStream($"{chatId.ToString()}.json",FileMode.Open);
        startFlag = false;
        var media = new InputMediaDocument(new InputMedia(fileStream, $"{chatId.ToString()}.json"));
        int mesId = botClient.GetChatAsync(chatId).Result.PinnedMessage.MessageId;
        botClient.EditMessageMediaAsync(
            chatId: chatId,
            messageId: mesId,
            media: media).Wait();
        fileStream.Dispose();
        System.IO.File.Delete($"{chatId.ToString()}.json");
        Message s = botClient.SendTextMessageAsync(chatId,"Session is over. Write anything to start using pseudoroot").Result;

    }

    private void RegisterNewUser()
    {
        using(StreamWriter fs = new StreamWriter($"{chatId.ToString()}.json"))
        {
            fs.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new Directory("rom",null)));  
        }
        RootDir = new Directory("rom", null);
        Console.WriteLine($"{RootDir == null}");
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
        closureTime = DateTime.Now.AddMinutes(3);
    }

   
    
}