using Telegram.Bot;
abstract class Command
{
    protected delegate string Handler(List<string> args, Session session);
    protected Handler Run;
    public Queue<Command>? CorrespondingCommands = null;
    public string? UserButtonAlias
    {
        protected set;
        get;
    }
    public string PerformingMessage
    {
        protected set;
        get;
    }   
    public bool RequiresArgument
    {
        get;
        protected set;
    } = false;
    
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
    
    public abstract string HandleDelegate(List<string> args, Session session);

    public void Handle(List<string> args, Session session)
    {
        try
        {
            Run(args, session);
            SendMessage(session,$"{session.Pwd.GetString}");
        }
        catch(Exception ex)
        {
            session.ChangeKeyboard(session.UserMenu);
            SendMessage(session, ex.ToString());
        }
    }
    public void SendMessage(Session session, string text)
    {
        session.BotClient.SendTextMessageAsync(
            chatId: session.ChatId,
            text: text,
            replyMarkup: session.Keyboard
        );
    }

    public void ForwardFile(Session session, Document doc)
    {
         session.BotClient.ForwardMessageAsync(
            chatId: session.ChatId,
            fromChatId: session.ChatId,
            messageId: doc.MessageId
        );
    }
}

