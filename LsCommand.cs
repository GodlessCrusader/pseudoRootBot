using System.Text;
using Telegram.Bot;
class LsCommand : Command
{
    public LsCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "ls";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(string cmdLn, FilePath pwd, string fileName, long chatId)
    {
        string jsonRepresent = "";
        using(StreamReader sr = new StreamReader(fileName))
            jsonRepresent = sr.ReadToEnd();
        Directory rootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(jsonRepresent);
        Directory current = Directory.GetDirectory(pwd, rootDir);
        StringBuilder stringBuilder = new StringBuilder();
        foreach(Directory d in current.ChildDirectories)
            stringBuilder.AppendLine($"|__{d.Name}");
        SendMessage(
            chatId,
            stringBuilder.ToString()
        );
        foreach(Document d in current.DocContents)
            ForwardFile(
                chatId,
                d
            );
        return null;
    }

}