using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
class SendMessageCommand : Command  
{
    List<Command>? rkm = null;
    public SendMessageCommand(TelegramBotClient botClient, CancellationToken ct, string messageText, List<Command> rkm)
    {
        this.rkm = rkm;
        this.BotClient = botClient;
        this.CancellationToken = ct;
        this.PerformingMessage = messageText;
        this.RequiresArgument = true;
    }
    public override string Handle(List<string> args, Session session)
    {
        session.ChangeKeyboard(rkm);
        SendMessage(session, this.PerformingMessage);
        return "";
    }
}