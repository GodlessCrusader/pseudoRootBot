class Directory : RootMember
{  
    public List<RootMember> contents = new List<RootMember>();
   
    public Directory(string name, Directory parent)
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
    // public void GetFileSystem(string tree)
    // {

    // }
}