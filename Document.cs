class Document{

public string Name{set; get; }

public string ParentDir{set; get; }
public int MessageId {
    set;
    get;
} = 0;
public Document(string name, Directory parent, int mesId){
    if(name!=null)
    {
        this.Name = name;
    }
    else
    {
        if(!parent.DocContents.Exists(x => x.Name == "new document"))
            this.Name = "new document";
        else
        {
            int i = 1;
            string nm = "new document";
            while(parent.DocContents.Exists(x => x.Name == nm))
            {
                nm = $"new document ({i++})";
            }
        }
    }
    this.ParentDir = parent.Name;
    this.MessageId= mesId;
}
}