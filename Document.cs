class Document : RootMember {

long MessageId {
    set;
    get;
} = 0;
public Document(string name, Directory parent, long mesId){
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