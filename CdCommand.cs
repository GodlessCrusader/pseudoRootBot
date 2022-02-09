class CdCommand : Command
{
    // protected new string name = "cd"; 
    public CdCommand()
    {
        base.Name = "cd";
    }

    public override void Handle(string cmdLn, FilePath pwd)
    {
        string[] args = cmdLn.Split(" ");
        if(args[1] == "..")
        {
            if(pwd.directories.Count>0)
            {
                // Console.WriteLine($"pwd Capacity:{pwd.directories.Capacity}");
                // foreach(string s in pwd.directories)
                // {
                //     Console.WriteLine($"elements of pwd.directories: {s}");
                // }
                pwd.directories.RemoveAt(pwd.directories.Count-1);
            }
            else
            {
                
            }
        }

        else //if(args[1] == "")
        {
            pwd.directories.Add(args[1]);
        }


    }
}