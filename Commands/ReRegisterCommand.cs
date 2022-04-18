using Telegram.Bot;
class ReRegisterCommand : Command
{
    public ReRegisterCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.UserButtonAlias = "Default settings(Reregister)";
        this.Name="reregister";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(List<string> args, Session session)
    {
       BotClient.UnpinAllChatMessages(session.ChatId);
       SendMessage(session, "Reopen your session for reregistration");
       return "";
    }
}