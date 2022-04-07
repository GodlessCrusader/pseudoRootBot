using Telegram.Bot;
class CloseCommand : Command
{
    public CloseCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "close";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(string cmdLn, Session session)
    {
        throw new NotImplementedException();
    }
}