using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
class SendMessageCommand : Command  
{
    List<Command>? rkm = null;
    public SendMessageCommand(string messageText, List<Command> rkm)
    {
        this.rkm = rkm;
        this.PerformingMessage = messageText;
        this.RequiresArgument = true;
        this.Run = HandleDelegate;
    }
    public override string HandleDelegate(List<string> args, Session session)
    {
        session.ChangeKeyboard(rkm);
        SendMessage(session, this.PerformingMessage);
        return "";
    }
}