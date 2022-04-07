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
    public override string Handle(string cmdLn, Session session)
    {
        Directory current = Directory.GetDirectory(session.Pwd, session.RootDir);
        StringBuilder stringBuilder = new StringBuilder();
        foreach(Directory d in current.ChildDirectories)
            stringBuilder.AppendLine($"|__{d.Name}");
        SendMessage(
            session.ChatId,
            stringBuilder.ToString()
        );
        foreach(Document d in current.DocContents)
            ForwardFile(
                session.ChatId,
                d
            );
        return null;
    }

}