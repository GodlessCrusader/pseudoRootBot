class Document : RootMember {

long MessageId {
    set;
    get;
}
public Document(string name, RootMember parent, long mesId){
    if(name!=null)
    {
        base.Name = name;
    }
    else
    {
        base.Name = "new document";
    }
    base.parentDir = parent;
    this.MessageId= mesId;
}
}