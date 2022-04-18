using Telegram.Bot;
class ChangeModeCommand : Command
{
    public ChangeModeCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "changemode";
        this.BotClient = botClient;
        this.CancellationToken = ct;
        this.UserButtonAlias = "Change interface mode";
    }
    public override string Handle(List<string> args, Session session)
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