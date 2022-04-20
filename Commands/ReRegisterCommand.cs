using Telegram.Bot;
class ReRegisterCommand : Command
{
    public ReRegisterCommand()
    {
        this.UserButtonAlias = "Default settings(Reregister)";
        this.Name="reregister";
        this.Run = HandleDelegate;
    }
    public override string HandleDelegate(List<string?> args, Session session)
    {
       session.BotClient.UnpinAllChatMessages(session.ChatId);
       SendMessage(session, "Reopen your session for reregistration");
       return "";
    }
}