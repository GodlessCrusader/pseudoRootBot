using Telegram.Bot;
abstract class Command
{   
    private TelegramBotClient bc;
    public TelegramBotClient BotClient
    {
        set{
            bc = value;
        }
        get{
            return bc;
        }
    }
    private CancellationToken ct;
    public CancellationToken CancellationToken
    {
        set{
            ct = value;
        }
        get{
            return ct;
        }
    }
    private string name = "";
    public string Name 
    {
        set{ 
            name = value;
        }
        get{
            return name;
        }
    }
    

    public abstract string Handle(string cmdLn, Session session);

    public void SendMessage(Session session, string text)
    {
        this.BotClient.SendTextMessageAsync(
            chatId: session.ChatId,
            text: text,
            cancellationToken: this.CancellationToken,
            replyMarkup: session.Keyboard
        );
    }

    public void ForwardFile(Session session, Document doc)
    {
         this.BotClient.ForwardMessageAsync(
            chatId: session.ChatId,
            fromChatId: session.ChatId,
            messageId: doc.MessageId,
            cancellationToken: this.CancellationToken
        );
    }
}
