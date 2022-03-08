class Document{

public string Name{set; get; }

public string ParentDir{set; get; }
long MessageId {
    set;
    get;
} = 0;
public Document(string name, string parent, long mesId){
    if(name!=null)
    {
        this.Name = name;
    }
    else
    {
        this.Name = "new document";
    }
    this.ParentDir = parent;
    this.MessageId= mesId;
}
}