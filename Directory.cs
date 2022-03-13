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
        Directory? currentDir = rootDir;
        foreach(string dirName in pwd.directories)
        {
            Console.WriteLine($"dirName1:{dirName}");
            if(currentDir.CheckExistance(dirName))
            {currentDir = currentDir.ChildDirectories.Find(x => x.Name == dirName);
            GetDirectory(pwd, currentDir);}

        }
        return currentDir;
    }
    
}
  