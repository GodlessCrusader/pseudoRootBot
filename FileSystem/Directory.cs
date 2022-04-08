class Directory
{  
    public string Name {get; set; }
    public string? parentDir {get; set; }
    public List<Directory> ChildDirectories = new List<Directory>();
    public List<Document> DocContents = new List<Document>();
   
    public Directory(string name, Directory? parent)
    {
        this.Name = name;
        if(parent!=null)
        {
            this.parentDir = parent.Name;
        }
    }

    public bool IsRootDirectory()
    {
        if (this.parentDir == null)
        {
            return true;
        } 
        else 
        {
            return false;
        }
    }

    public bool CheckExistance(string name)
    { 
        foreach(Directory rm in this.ChildDirectories)
        {
            if(rm.Name == name ) //&& rm is Directory
            {
                return true;
            }
        }

        foreach(Document rm in this.DocContents)
        {
            if(rm.Name == name ) //&& rm is Directory
            {
                return true;
            }
        }
        
        return false;
    }

    public static Directory? GetDirectory(FilePath pwd, Directory rootDir)
    {
        Directory curDir = rootDir;
        FilePath p = pwd;
        int i = 0;
        while(i < p.directories.Count)
        {
            Console.WriteLine($"p[i] {p.directories[i]} i {i.ToString()}");
            if(curDir.ChildDirectories.Exists(x => x.Name == p.directories[i]))
            {
                curDir = curDir.ChildDirectories.Find(x => x.Name == p.directories[i]);
                i++;
            }
            else
            {
                throw new Exception("getdir error");
            }
        }
        return curDir;
        // Directory result = rootDir;
        // Console.WriteLine($"pwd count {pwd.directories.Count} i {j}");
        // for(int i = j; i<pwd.directories.Count; i++)
        // {
        //     if(rootDir.CheckExistance(pwd.directories[i]))
        //     {
        //         result = GetDirectory(pwd, rootDir.ChildDirectories.Find(x => x.Name == pwd.directories[i]), ++i);
        //     }
        //     else
        //     {
        //         throw new Exception("GetDIr");
        //     }

        // }
        // return result;
    }
    
}
  