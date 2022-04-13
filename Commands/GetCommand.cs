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
    public override string Handle(string cmdLn, Session session)
    {
        var cmds = cmdLn.Split(' ');
        var current = Directory.GetDirectory(session.Pwd, session.RootDir);
        ForwardFile(session, current.DocContents.Find(x => x.Name == cmds[1]));
        return "";
    }
}