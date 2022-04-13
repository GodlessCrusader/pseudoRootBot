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

    public static void Handle(Session session, string? documentName, int mesId)
    {
        Directory current = Directory.GetDirectory(session.Pwd, session.RootDir);
        if(current.DocContents.Exists(x => x.Name == documentName))
        {
            throw new Exception("File with this name already exists");
        }
        else
        {
            current.DocContents.Add(new Document(documentName, current, mesId));
        }
    }
}   