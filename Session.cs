using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
class Session 
{
    
    public Command? IsPerforming = null; 
    public List<string> performingArgs = new List<string>();
    public UserPreferences userPreferences = new UserPreferences();
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
    public ITelegramBotClient BotClient
    {
        get
        {
            return botClient;
        }
    }
    private Telegram.Bot.Types.File? fileSystemDoc;
    public Telegram.Bot.Types.File? FileSystemDoc
    {
        get{
            return fileSystemDoc;
        }
    }
    private FilePath pwd = new FilePath();

    private ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton("start")){ResizeKeyboard = true};


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

    public List<Command> CommandList
    {
        get;
        private set;
    }= new List<Command>() 
    {
        new CdCommand(),
        new MkdirCommand(),
        new DeleteCommand(),
        new TreeCommand(),
        // new CreateCommand(),
        new LsCommand(),
        new GetCommand(),
        // new ChangeModeCommand(),
        new ReRegisterCommand(),
        new RenameCommand(),
        new ShowCommandsCommand(),
        new CreateCommand()
    };

    // public List<Command> UserMenu = new List<Command>()
    // {
    //     new ShowCommandsCommand()
    // };

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
        ses.ChangeKeyboard(null);
        botClient.SendTextMessageAsync(chatId,ses.Pwd.GetString(),replyMarkup: ses.Keyboard);
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

            userPreferences = Newtonsoft.Json.JsonConvert.DeserializeObject<UserPreferences>(jr);
            Console.WriteLine($"RootDir.ToString() {RootDir == null}");
            RootDir = userPreferences.RootDir;
            Console.WriteLine($"RootDir.ToString() {RootDir == null}");
           
        }
        System.IO.File.Delete($"{chatId.ToString()}.json");
        
    }

    public void Close()
    {
        if(botClient.GetChatAsync(chatId).Result.PinnedMessage != null)
        {
            // System.IO.File.Create($"{chatId.ToString()}.json");
            StreamWriter sw = new StreamWriter(($"{chatId.ToString()}.json"));
            string jr = Newtonsoft.Json.JsonConvert.SerializeObject(this.userPreferences);
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
            Message s = botClient.SendTextMessageAsync(
                chatId,
                "Session is over. Write anything to start using pseudoroot",
                replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("Start"))).Result;

        }
    }

    private void RegisterNewUser()
    {
        using(StreamWriter fs = new StreamWriter($"{chatId.ToString()}.json"))
        {
            fs.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new UserPreferences()));  
        }
        userPreferences = new UserPreferences();
        // RootDir = new Directory("rom", null);
        Console.WriteLine($"{RootDir == null}");
        Message m = new Message();
        using(FileStream transfer = System.IO.File.Open($"{chatId.ToString()}.json", FileMode.OpenOrCreate))
        {
            m = botClient.SendDocumentAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(transfer),null,$"{chatId.ToString()}.json").Result;
            botClient.PinChatMessageAsync(chatId,m.MessageId);
            fileSystemDoc = botClient.GetFileAsync(m.Document.FileId).Result;
        }
        System.IO.File.Delete($"{chatId.ToString()}.json");

    }

    public void ResetTimeout(int time)
    {
        closureTime = DateTime.Now.AddMinutes(3);
    }

   public void ChangeKeyboard(List<Command>? lc, bool showContents = true)
    {
        var current = Directory.GetDirectory(this.Pwd, this.RootDir);
        int c;
        if(lc == null)
            c = current.ChildDirectories.Count + current.DocContents.Count + 1;
        else if(!showContents)
            c = lc.Count;
        else
            c = current.ChildDirectories.Count + current.DocContents.Count + lc.Count;
        if(c>0)
        {
            KeyboardButton[][] we = new KeyboardButton[c][];
            for(int z = 0; z < c;z++)
            {
                we[z] = new KeyboardButton[3];
                for(int o = 0;o<3;o++)
                    we[z][o] = new KeyboardButton("");
            }
            int i = 0;
            int j = 0;
            if(showContents)
            {
                i++;
                we[0][0].Text = "⤴️..";
                we[0][1].Text = "❌";
                we[0][2].Text = new ShowCommandsCommand().UserButtonAlias;
                foreach(Directory d in current.ChildDirectories)
                {
                    if(j<3)
                    {
                        we[i][j].Text = $"📁{d.Name}";
                        j++;
                    }
                    else
                    {
                        // we.Append(new KeyboardButton[3]{$"📁{d.Name}", "", ""});
                        i++;
                        if(i<c)
                        {
                            we[i][0].Text = $"📁{d.Name}";
                            j = 1;
                        }
                    }
                    
                }
                foreach(Document d in current.DocContents)
                {
                    if(j<3)
                    {
                        we[i][j].Text = $"📄{d.Name}";
                        j++;
                    }
                    else
                    {
                        if(i<c)
                        {
                            // we.Append(new KeyboardButton[3]{$"📄{d.Name}", new(""), new("")});
                            i++;
                            we[i][0].Text = $"📄{d.Name}";
                            j = 1;
                        }
                    }
                    
                }
            }
            if(lc != null)
            {
                foreach(Command d in lc)
                {
                    if(d.UserButtonAlias != null)
                    {
                        if(j<3)
                        {
                            Console.WriteLine($"i:{i}");
                            we[i][j].Text = d.UserButtonAlias;
                            j++;
                        }
                        else
                        {
                            if(i<c)
                            {
                                // we.Append(new KeyboardButton[3]{d.Name, "", ""});
                                i++;
                                we[i][0].Text = d.UserButtonAlias;
                                j = 1;
                            }
                        }
                    }
                    
                }     
            } 
            this.Keyboard.Keyboard = we; 
        }  
        
        Console.WriteLine("Keyboard change is complete");
    }
    
}