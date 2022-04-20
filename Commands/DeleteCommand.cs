using Telegram.Bot;

class DeleteCommand : Command {

    public DeleteCommand()
    {
        this.UserButtonAlias = "Delete";
        this.PerformingMessage = "Enter the name of delete target";
        this.RequiresArgument = true;
        this.Name = "delete";
        this.Run = HandleDelegate;
    }
    public override string HandleDelegate(List<string?> args, Session session)
    {
        var current = Directory.GetDirectory(session.Pwd, session.RootDir);
        var target = PseudoRoot.AliesTranslation.TranslateKeyboardArg(args[1], current);
        if(target is Directory)
            current.ChildDirectories.Remove((Directory)target);
        if(target is Document)
            current.DocContents.Remove((Document)target);
        session.ChangeKeyboard(null);
        return "";
    }
}