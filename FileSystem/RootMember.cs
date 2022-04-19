class RootMember {
    public string Name {
        set;
        get;
    } = "rom";
    public string? ParentDir
    {set; get;} = "";
   
    public RootMember()
    {

    }
    // public RootMember(string name, RootMember parent)
    // {
    //     this.Name = name;
    //     this.parentDir = parent;
    // }
    // public void GetFileSystem(string tree)
    // {

    // }
}
