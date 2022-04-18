using Telegram.Bot;
class GetCommand : Command
{
    public GetCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.RequiresArgument = true;
        this.Name = "get";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(List<string> args, Session session)
    {
        var current = Directory.GetDirectory(session.Pwd, session.RootDir);
        ForwardFile(session, current.DocContents.Find(x => x.Name == args[1]));
        return "";
    }
}