using Telegram.Bot;
class CreateCommand : Command
{
    public CreateCommand()
    {
        this.Name = "create";
    }

    public override string HandleDelegate(List<string> args, Session session)
    {
        throw new NotImplementedException();
    }

    public static void Handle(Session session, string? documentName, int mesId)
    {
        Directory current = Directory.GetDirectory(session.Pwd, session.RootDir);
        if(current.DocContents.Exists(x => x.Name == documentName))
        {
            throw new Exception("File with this name already exists");
        }
        else
        {
            current.DocContents.Add(new Document(documentName, current, mesId));
        }
    }
}   