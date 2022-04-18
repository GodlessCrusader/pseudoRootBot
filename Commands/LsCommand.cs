using System.Text;
using Telegram.Bot;
class LsCommand : Command
{
    public LsCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.UserButtonAlias = "Show contents";
        this.Name = "ls";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(List<string> args, Session session)
    {
        Directory current = Directory.GetDirectory(session.Pwd, session.RootDir);
        StringBuilder stringBuilder = new StringBuilder();
        foreach(Directory d in current.ChildDirectories)
            stringBuilder.AppendLine($"|__{d.Name}");
        SendMessage(
            session,
            stringBuilder.ToString()
        );
        foreach(Document d in current.DocContents)
            ForwardFile(
                session,
                d
            );
        return null;
    }

}