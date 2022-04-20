using Telegram.Bot;
class GetCommand : Command
{
    public GetCommand()
    {
        this.RequiresArgument = true;
        this.Name = "get";
        this.Run = HandleDelegate;
    }
    public override string HandleDelegate(List<string?> args, Session session)
    {
        var current = Directory.GetDirectory(session.Pwd, session.RootDir);
        ForwardFile(session, current.DocContents.Find(x => x.Name == args[1]));
        session.ChangeKeyboard(null);
        return "";
    }
}