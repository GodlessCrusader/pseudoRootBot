using Telegram.Bot;
class CreateCommand : Command
{
    public CreateCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "create";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }

    public override string Handle(string cmdLn, FilePath pwd, string fileName, long chatId)
    {
        throw new NotImplementedException();
    }

    public static string Handle(FilePath pwd, string fileJsonName, string documentName, int mesId)
    {
        string jsonRepresent = "";
        
        using(StreamReader sr = new StreamReader(fileJsonName))
            jsonRepresent = sr.ReadToEnd();
        
        Directory rootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(jsonRepresent);
        Directory current = Directory.GetDirectory(pwd, rootDir);
        current.DocContents.Add(new Document(documentName, current.Name, mesId));
        jsonRepresent = Newtonsoft.Json.JsonConvert.SerializeObject(rootDir);

        using(StreamWriter sw = new StreamWriter(fileJsonName))
            sw.Write(jsonRepresent);
        
        return null;
    }
}   