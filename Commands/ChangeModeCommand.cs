using Telegram.Bot;
class ChangeModeCommand : Command
{
    public ChangeModeCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "changemode";
        this.UserButtonAlias = "Change interface mode";
        this.Run = HandleDelegate;
    }
    public override string HandleDelegate(List<string> args, Session session)
    {
        if(session.PreferedMode == CommandMode.InternalKeyboard)
        {
            session.PreferedMode = CommandMode.BashConsole;
        }
        else
        {
            session.PreferedMode = CommandMode.InternalKeyboard;
        }
            
        return "";
    }
}