abstract class Command
{
    static List<Command> cmdList = new List<Command>();

    private string name = "";
    public string Name 
    {
        set{ 
            name = value;
        }
        get{
            return name;
        }
    }
    

    public abstract void Handle(string cmdLn, FilePath pwd, string fileName);
}

