


class MkdirCommand : Command
{
    public MkdirCommand()
    {
        this.Name = "mkdir";
    }
    public override void Handle(string cmdLn, FilePath pwd, string fileName)
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
                string jsonRepresent = "";
                using(StreamReader str = new StreamReader(fileName))
                {
                    jsonRepresent = str.ReadToEnd();
                }
                Directory? rootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(jsonRepresent);
                Directory? current;
                if(rootDir!=null)
                {
                    current = Directory.GetDirectory(pwd.directories[pwd.directories.Count-1],pwd.directories[pwd.directories.Count - 2], rootDir);
                }
                else
                {
                    current = null;
                }

                if(current!=null)
                {
                    
                    if(current!.CheckExistance(args[1]))
                    {
                        throw new Exception($"Directory {args[1]} already exists");
                    }
                    else
                    {
                        current.contents.Add(new Directory(args[1], current));
                        jsonRepresent = Newtonsoft.Json.JsonConvert.SerializeObject(rootDir);
                        using(StreamWriter sw = new StreamWriter(fileName))
                        {
                            sw.Write(jsonRepresent);
                        }
                    }                        
                }
            
            }   
        }
    }
    // private bool forbidenSymsCheck()
    // {
    //     if()
    //     return false;
    // }
    private const string forbiddenSyms = @"""/\,:.?#%&{}$!'@<>* +|=";
}