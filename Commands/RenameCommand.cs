using Telegram.Bot;
class RenameCommand : Command   
{
    public RenameCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "rename";
        this.BotClient = botClient;
        this.CancellationToken = ct;
        this.PerformingMessage = "Select target element";
        this.RequiresArgument = true;
        this.UserButtonAlias = "Rename";
        this.CorrespondingCommands = new Queue<Command>();
        CorrespondingCommands.Enqueue(new SendMessageCommand(botClient, ct, "Enter new name", null));
        Console.WriteLine($"Corresponding command count construction: {CorrespondingCommands.Count}");
    }
    public override string Handle(List<string> args, Session session)
    {
        var current = Directory.GetDirectory(session.Pwd, session.RootDir);
        
        Console.WriteLine($"Inner args state: {String.Join(" ", args)}");

        if(current.ChildDirectories.Exists(x => x.Name == args[1]))
            current.ChildDirectories.Find(x => x.Name == args[1]).Name = args[2];
        else if(current.DocContents.Exists(x => x.Name == args[1]))
            current.DocContents.Find(x => x.Name == args[1]).Name = args[2];
        else 
            throw new Exception("Directory or document with such name doesn't exist");
         CorrespondingCommands.Enqueue(new SendMessageCommand(BotClient, CancellationToken, "Enter new name", null));
        return "";
    }
}