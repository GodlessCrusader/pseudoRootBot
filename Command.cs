abstract class Command
{
    // static List<Command> wordsList = new List<Command>();

    private string name;
    public string Name 
    {
        set{
            name = value;
        }
        get{
            return name;
        }
    }

    public abstract void Handle();
}