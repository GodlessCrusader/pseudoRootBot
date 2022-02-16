abstract class RootMember {
    public Directory? parentDir;
    public string Name {
        set;
        get;
    } = "rom";

    // public RootMember(string name, RootMember parent)
    // {
    //     this.Name = name;
    //     this.parentDir = parent;
    // }
    // public void GetFileSystem(string tree)
    // {

    // }
}
