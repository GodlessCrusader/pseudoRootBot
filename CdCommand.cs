class CdCommand : Command
{

    public CdCommand()
    {
        this.Name = "cd";
    }

    public override void Handle(string cmdLn, FilePath pwd, string fileName)
    {
        List<string> args = cmdLn.Split(" ").ToList();
        if(args.Count > 1)
        {
            if(args[1] == "..")
            {
                if(pwd.directories.Count>0)
                {
                    // Console.WriteLine($"pwd Capacity:{pwd.directories.Capacity}");
                    // foreach(string s in pwd.directories)
                    // {
                    //     Console.WriteLine($"elements of pwd.directories: {s}");
                    // }
                    pwd.directories.RemoveAt(pwd.directories.Count-1);
                }
                else
                {
                    
                }
            }

            else 
            {
                List<string> cdPath = args[1].Split(@"\").ToList<string>();
                using(StreamReader sr = new StreamReader(fileName))
                {
                    Directory? rootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(sr.ReadToEnd());
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
                        foreach(string s in cdPath)
                        {
                            if(current!.CheckExistance(s))
                            {
                                current = current.contents.Find(x => x.Name == s) as Directory;
                                //pwd.directories.Add(s);
                            }
                            else
                            {
                                break;//user recieves error message
                            }
                        }
                        if(current.Name == cdPath[cdPath.Count-1])
                        {
                            pwd.directories = pwd.directories.Concat(cdPath).ToList();
                        }
                    }
                    //pwd.directories.Add(args[1]);
                }
            }
        }


    }
}