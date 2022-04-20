using Telegram.Bot;
class CreateCommand : Command
{
    public CreateCommand()
    {
        this.Name = "create";
        this.Run = HandleDelegate;
    }

    public override string HandleDelegate(List<string?> args, Session session)
    {

        Directory current = Directory.GetDirectory(session.Pwd, session.RootDir);
        if(current.DocContents.Exists(x => x.Name == args[0]))
        {
            throw new Exception("File with this name already exists");
        }
        else
        {
            current.DocContents.Add(new Document(args[0], current, Int32.Parse(args[1])));
        }
        session.ChangeKeyboard(null);
        return"";
    }

    // public static void Handle(Session session, string? documentName, int mesId)
    // {
    //     Directory current = Directory.GetDirectory(session.Pwd, session.RootDir);
    //     if(current.DocContents.Exists(x => x.Name == documentName))
    //     {
    //         throw new Exception("File with this name already exists");
    //     }
    //     else
    //     {
    //         current.DocContents.Add(new Document(documentName, current, mesId));
    //     }
    // }
}   