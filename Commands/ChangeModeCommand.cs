using Telegram.Bot;
class ChangeModeCommand : Command
{
    public ChangeModeCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "changemode";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(string cmdLn, Session session)
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