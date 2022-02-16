class CdCommand : Command
{
    // protected new string name = "cd"; 
    public CdCommand()
    {
        base.Name = "cd";
    }

    public override void Handle(string cmdLn, FilePath pwd)
    {
        string[] args = cmdLn.Split(" ");
        if(args.Length > 1)
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
                using(StreamReader sr = new StreamReader("testFS.json"))
                {
                    Directory? rootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(sr.ReadToEnd());
                    Directory current = Directory.GetDirectory(pwd.directories[pwd.directories.Count],pwd.directories[pwd.directories.Count - 1], rootDir);
                    //pwd.directories.Add(args[1]);
                }
            }
        }


    }
}