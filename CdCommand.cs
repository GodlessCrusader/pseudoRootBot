using Telegram.Bot;

class CdCommand : Command
{

    public CdCommand(TelegramBotClient botClient, CancellationToken ct)
    {
        this.Name = "cd";
        this.BotClient = botClient;
        this.CancellationToken = ct;
    }

    public override string Handle(string cmdLn, Session session)
    {
        List<string> args = cmdLn.Split(" ").ToList();
        if(args.Count > 1)
        {
            if(args[1] == "..")
            {
                if(session.Pwd.directories.Count>0)
                {
                    session.Pwd.directories.RemoveAt(session.Pwd.directories.Count-1);
                }
                else
                {
                    throw new Exception("Working directory is root directory. Can't go up");
                }
            }

            else 
            {
                List<string> cdPath = args[1].Split(@"\").ToList<string>();
                foreach(string s in cdPath)
                {
                    s.Replace(" ", "");
                }


                Directory? current;

                if(session.RootDir!=null)
                {
                    if(session.Pwd.directories.Count>=1)
                    {
                        current = Directory.GetDirectory(session.Pwd, session.RootDir);
                    }
                    else
                    {
                        current = session.RootDir;   
                    }
                }
                else
                {
                    current = null;
                    
                }
               
                if(current!=null)
                {
                    foreach(string s in cdPath)
                    {
                        
                        if(current!.CheckExistance(s))
                        {
                            Console.WriteLine($"s var: {s}");
                            foreach(Directory rm in current.ChildDirectories)
                            Console.WriteLine($"current child directories: {rm.Name}");
                            current = current.ChildDirectories.Find(x => x.Name == s);
                            Console.WriteLine($"Current var: {current.Name}");
                            
                        }
                        else
                        {
                            throw new Exception($"Destination point: {args[1]} does not exist");
                        }
                    }
                    if(current.Name == cdPath[cdPath.Count-1])
                    {
                        session.Pwd.directories = session.Pwd.directories.Concat(cdPath).ToList();
                    }
                }
                else
                {
                    throw new Exception($"Destination point: {args[1]} does not exist");
                }
                
            }
        }

        return null!;
    }
}