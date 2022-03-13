class DeleteCommand : Command {

    public DeleteCommand()
    {
        this.Name = "delete";
    }
    public override string Handle(string cmdLn, FilePath pwd, string fileName)
    {
        var args = cmdLn.Split(" ").ToList();
        if(args.Capacity>1)
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
                    current.ChildDirectories.Remove(current.ChildDirectories.Find(x => x.Name == args[1])!);
                    jsonRepresent = Newtonsoft.Json.JsonConvert.SerializeObject(rootDir);
                    using(StreamWriter sw = new StreamWriter(fileName))
                    {
                        sw.Write(jsonRepresent);
                    }
                }
                else
                {
                    throw new Exception($"Directory {args[1]} doesn't exists");
                   
                }                        
            }
            else
            {
                throw new Exception($"Directory {args[1]} doesn't exists");
                // rootDir = new Directory("rom", null);
                // rootDir.ChildDirectories.Add(new Directory(args[1], rootDir));
                //     jsonRepresent = Newtonsoft.Json.JsonConvert.SerializeObject(rootDir);
                //     using(StreamWriter sw = new StreamWriter(fileName))
                //     {
                //         sw.Write(jsonRepresent);
                //     }
            }
            
        }
        return null!;
    }
}