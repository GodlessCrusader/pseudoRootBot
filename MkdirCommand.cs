


class MkdirCommand : Command
{
    public MkdirCommand()
    {
        this.Name = "mkdir";
    }
    public override string Handle(string cmdLn, FilePath pwd, string fileName)
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
                Directory? current = null;
                if(rootDir!=null)
                {
                    if(pwd.directories.Count>1)
                    {
                        //bugged need a way to define directory correctly
                        current = Directory.GetDirectory(pwd, rootDir);
                    }
                    if(pwd.directories.Count == 0)
                    {
                        current = rootDir;
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
                        jsonRepresent = Newtonsoft.Json.JsonConvert.SerializeObject(rootDir);
                        using(StreamWriter sw = new StreamWriter(fileName))
                        {
                            sw.Write(jsonRepresent);
                        }
                    }                        
                }
                else
                {
                    rootDir = new Directory("rom", null);
                    rootDir.ChildDirectories.Add(new Directory(args[1], rootDir));
                        jsonRepresent = Newtonsoft.Json.JsonConvert.SerializeObject(rootDir);
                        using(StreamWriter sw = new StreamWriter(fileName))
                        {
                            sw.Write(jsonRepresent);
                        }
                }
            }   
        }
        return null!;
    }
    // private bool forbidenSymsCheck()
    // {
    //     if()
    //     return false;
    // }
    private const string forbiddenSyms = @"""/\,:.?#%&{}$!'@<>* +|=";
}