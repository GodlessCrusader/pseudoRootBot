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

    public void SendMessage(long chatId, string text)
    {
        this.BotClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            cancellationToken: this.CancellationToken
        );
    }

    public void ForwardFile(long chatId, Document doc)
    {
         this.BotClient.ForwardMessageAsync(
            chatId: chatId,
            fromChatId: chatId,
            messageId: doc.MessageId,
            cancellationToken: this.CancellationToken
        );
    }
}

