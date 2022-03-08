class TreeCommand : Command {

    public TreeCommand()
    {
        this.Name = "tree";
    }
    public override string Handle(string cmdLn, FilePath pwd, string fileName)
    {
        string? tree = "rom";

        Directory? rootDir = null;

        using(StreamReader sr = new StreamReader(fileName))
        {
            rootDir=Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(sr.ReadToEnd());
        }

        if(rootDir != null)
        {
            
        }
        return tree!;
    }
}