using PseudoRoot;
using Telegram.Bot;
class RenameCommand : Command   
{
    public RenameCommand()
    {
        this.Run = HandleDelegate;
        this.Name = "rename";
        this.PerformingMessage = "Select target element";
        this.RequiresArgument = true;
        this.UserButtonAlias = "Rename";
        this.CorrespondingCommands = new Queue<Command>();
        CorrespondingCommands.Enqueue(new SendMessageCommand("Enter new name", null));
        Console.WriteLine($"Corresponding command count construction: {CorrespondingCommands.Count}");
    }
    public override string HandleDelegate(List<string?> args, Session session)
    {
        CorrespondingCommands.Enqueue(new SendMessageCommand("Enter new name", null));
        var current = Directory.GetDirectory(session.Pwd, session.RootDir);
        
        Console.WriteLine($"Inner args state: {String.Join(" ", args)}");

        var target = AliesTranslation.TranslateKeyboardArg(args[1],current);

        if(current.CheckExistance(args[2]))
            throw new Exception("Directory with such name already exists");

        target.Name = args[2];

        session.ChangeKeyboard(null);
        return "";
    }
}