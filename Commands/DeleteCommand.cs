using Telegram.Bot;

class DeleteCommand : Command {

    public DeleteCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.PerformingMessage = "Enter the name of delete target";
        this.RequiresArgument = true;
        this.Name = "delete";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(string cmdLn, Session session)
    {
        var args = cmdLn.Split(" ").ToList();
        if(args.Count>1)
        {
            Directory? current = null;
            if(session.RootDir!=null)
            {
                if(session.Pwd.directories.Count>1)
                {
                    current = Directory.GetDirectory(session.Pwd, session.RootDir);
                }
                if(session.Pwd.directories.Count == 0)
                {
                    current = session.RootDir;
                }
            }
            if(current!=null)
            {
                
                if(current!.CheckExistance(args[1]))
                {
                    current.ChildDirectories.Remove(current.ChildDirectories.Find(x => x.Name == args[1])!);
                }
                else
                {
                    throw new Exception($"Directory {args[1]} doesn't exists");
                   
                }                        
            }
            else
            {
                throw new Exception($"Directory {args[1]} doesn't exists");
            }
            
        }
        return null!;
    }
}