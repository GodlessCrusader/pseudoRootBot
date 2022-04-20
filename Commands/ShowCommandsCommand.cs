using Telegram.Bot;
class ShowCommandsCommand : Command
{
    public ShowCommandsCommand()
    {
        this.Name = "showcommands";
        this.UserButtonAlias = "Options";
        this.Run = HandleDelegate;
    }
    public override string HandleDelegate(List<string?> args, Session session)
    {
        session.ChangeKeyboard(new List<Command>(){
            new MkdirCommand(),
            new DeleteCommand(),
            new TreeCommand(),
            new LsCommand(),
            new ReRegisterCommand(),
            new RenameCommand()
        }, false);
        return"";
    }
}