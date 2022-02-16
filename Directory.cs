class Directory : RootMember
{  
    public List<RootMember> contents = new List<RootMember>();
   
    public Directory(string name, Directory? parent)
    {
        this.Name = name;
        this.parentDir = parent;
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
        
        
        foreach(RootMember rm in this.contents)
        {
            if(rm.Name == name && rm is Directory)
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
            foreach(RootMember rm in rootDir.contents)
            {
                if(rm is Directory && rm.Name == dirName)
                {
                    return rm as Directory;
                }
            }
        }
        else
        {
            foreach(RootMember rm in rootDir.contents)
            {
                if (rm is Directory)
                {
                    Directory? temp = rm as Directory;
                    if(temp != null && GetDirectory(dirName, parentName, temp) != null)
                    {
                        return GetDirectory(dirName, parentName, temp);
                    }
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
  