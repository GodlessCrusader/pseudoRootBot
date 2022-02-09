class CdCommand : Command
{
    public override void Handle()
    {
        System.Console.WriteLine("no args");
    }
    public void Handle(string cmdLn)
    {
        string[] args = cmdLn.Split(" ");
        if(args[1] == "..")
        {
            
        }
    }
}