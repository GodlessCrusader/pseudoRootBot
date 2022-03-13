class CdCommand : Command
{

    public CdCommand()
    {
        this.Name = "cd";
    }

    public override string Handle(string cmdLn, FilePath pwd, string fileName)
    {
        List<string> args = cmdLn.Split(" ").ToList();
        if(args.Count > 1)
        {
            if(args[1] == "..")
            {
                if(pwd.directories.Count>0)
                {
                    pwd.directories.RemoveAt(pwd.directories.Count-1);
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
                using(StreamReader sr = new StreamReader(fileName))
                {
                    Directory? rootDir = Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(sr.ReadToEnd());
                    foreach(var c in rootDir!.ChildDirectories)
                    {Console.WriteLine($"cont: {c.Name}");}
                    Directory? current;
                    if(rootDir!=null)
                    {
                        Console.WriteLine("Entered rootDir not null check");
                        if(pwd.directories.Count>=1)
                        {
                            Console.WriteLine("Entered pwd count >=1 check");
                            current = Directory.GetDirectory(pwd, rootDir);
                        }
                        else
                        {
                            Console.WriteLine("Entered pwd count <=1 check");
                            Console.WriteLine("Entered pwd count <1 check");
                            current = rootDir;
                            
                        }
                    }
                    else
                    {
                        current = null;
                        
                    }
                    Console.WriteLine($"Current var: {current}");
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
                                Console.WriteLine($"Current var: {current}");
                                
                            }
                            else
                            {
                                throw new Exception($"Destination point: {args[1]} does not exist");
                            }
                        }
                        if(current.Name == cdPath[cdPath.Count-1])
                        {
                            pwd.directories = pwd.directories.Concat(cdPath).ToList();
                        }
                    }
                    else
                    {
                        throw new Exception($"Destination point: {args[1]} does not exist");
                    }
                }
            }
        }

        return null!;
    }
}