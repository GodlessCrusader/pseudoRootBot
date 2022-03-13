
using System.Text;
class TreeCommand : Command {

    public TreeCommand()
    {
        this.Name = "tree";
    }
    public override string Handle(string cmdLn, FilePath pwd, string fileName)
    {
        StringBuilder tree = new StringBuilder();

        Directory? rootDir = null;

        using(StreamReader sr = new StreamReader(fileName))
        {
            rootDir=Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(sr.ReadToEnd());
        }

        if(rootDir != null)
        {
           return DrawFileTree(rootDir,tree, 0, 0).ToString();
        }
        
        return tree.ToString();
    }

    public StringBuilder DrawFileTree(Directory dir, StringBuilder tree, int iter, int lastCount)
    {
        iter++;
        tree.AppendLine(dir.Name);
        // for(int i=0; i<dir.Name.Length-1;i++)
        // {
        //     tree.Append(" ");
        // }
        foreach(Directory d in dir.ChildDirectories)
        {
            if(dir.parentDir == null)
            {
                if(dir.ChildDirectories[dir.ChildDirectories.Count-1] == d)
                {
                lastCount++;
                for(int i = 0; i<lastCount;i++)
                {
                    tree.Append("  ");
                }
                for(int i = lastCount; i<iter;i++)
                {
                    tree.Append("  |");
                }
                tree.Append("|___");
                }
                else
                {
                lastCount++;
                for(int i = 0; i<lastCount;i++)
                {
                    tree.Append("|  ");
                }
                for(int i = lastCount; i<iter;i++)
                {
                    tree.Append("  |");
                }
                tree.Append("|___");
                }

            }
            else if( lastCount > 0)
            {
                string s = "___";
                if(dir.ChildDirectories[dir.ChildDirectories.Count-1] == d)
                {
                    lastCount++;
                    s = "|___";
                }
                for(int i = 0; i<lastCount;i++)
                {
                    tree.Append("  ");
                }
                for(int i = lastCount; i<iter;i++)
                {
                    tree.Append("  |");
                }
                tree.Append(s);
            }
            else
            {
                for(int i = 0; i<iter;i++)
                {
                    tree.Append("  |");
                }
                tree.Append("___");
                
            }
           
            DrawFileTree(d, tree, iter, lastCount);
        }
        return tree;
    }

}