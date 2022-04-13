

using Telegram.Bot;

class MkdirCommand : Command
{
    public MkdirCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.PerformingMessage = "Enter the name of a new directory";
        this.RequiresArgument = true;
        this.Name = "mkdir";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }
    public override string Handle(string cmdLn, Session session)
    {
        var args = cmdLn.Split(" ").ToList();
        if(args.Capacity>1)
        {
            if(args[1].Intersect(forbiddenSyms).Count()>1)
            {
                throw new Exception($"Directory name contains forbidden symbols: {forbiddenSyms}");
                //error message
            }
            else
            {
                Directory? current = null;
                if(session.RootDir!=null)
                {
                    if(session.Pwd.directories.Count>=1)
                    {
                        //bugged need a way to define directory correctly
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
                        throw new Exception($"Directory {args[1]} already exists");
                    }
                    else
                    {
                        current.ChildDirectories.Add(new Directory(args[1], current));
                    }                        
                }
            }   
        }
        return null!;
    }
    private const string forbiddenSyms = @"""/\,:.?#%&{}$!'@<>* +|=";
}