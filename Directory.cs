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

    public static Directory? GetDirectory(string dirName, string parentName, Directory rootDir)
    {

        if(rootDir.Name == parentName)
        {
            foreach(Directory rm in rootDir.ChildDirectories)
            {
                if(rm.Name == dirName)
                {
                    return rm;
                }
            }
        }
        else
        {
            foreach(Directory rm in rootDir.ChildDirectories)
            {
                if(GetDirectory(dirName, parentName, rm) != null)
                {
                    return GetDirectory(dirName, parentName, rm);
                }
                
            }
        }
        return null;
    }

    // public bool CheckExistance(string dir, string parent)
    // {
    //     if(this.Name == dir)
    //     {   
    //         return true;                                                                           awaits for the search method
    //     }

    //     else if(this.Name == parent)
    //     {
    //         foreach(RootMember rm in this.contents)
    //         {
    //             if(rm.Name == dir)
    //             {
    //                 return true;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         foreach(RootMember rm in this.contents)
    //         {
    //             if(rm is Directory)
    //             {
    //                 var temp = rm as Directory;
    //                 if(temp.CheckExistance(dir,parent))
    //                 {
    //                     return true;
    //                 }
    //             }
    //         }
    //     }
    //     return false;
}
  