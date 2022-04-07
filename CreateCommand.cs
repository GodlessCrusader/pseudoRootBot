using Telegram.Bot;
class CreateCommand : Command
{
    public CreateCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "create";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }

    public override string Handle(string cmdLn, Session session)
    {
        throw new NotImplementedException();
    }

    public static void Handle(FilePath pwd, string fileJsonName, string? documentName, int mesId)
    {
        string jsonRepresent = "";
        
        using(StreamReader sr = new StreamReader(fileJsonName))
            jsonRepresent = sr.ReadToEnd();
        
        Directory rootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(jsonRepresent);
        Directory current = Directory.GetDirectory(pwd, rootDir);
        if(current.DocContents.Exists(x => x.Name == documentName))
        {

        }
        else
        {
            current.DocContents.Add(new Document(documentName, current, mesId));
        }
        jsonRepresent = Newtonsoft.Json.JsonConvert.SerializeObject(rootDir);

        using(StreamWriter sw = new StreamWriter(fileJsonName))
            sw.Write(jsonRepresent);
    }
}   