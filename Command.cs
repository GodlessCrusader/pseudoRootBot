abstract class Command
{   
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
    

    public abstract string Handle(string cmdLn, FilePath pwd, string fileName);
}

