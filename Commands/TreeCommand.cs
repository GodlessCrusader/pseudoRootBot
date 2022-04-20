
using System.Text;
using Telegram.Bot;

class TreeCommand : Command {

    public TreeCommand()
    {
        this.UserButtonAlias = "Show file tree";
        this.Name = "tree";
        this.Run = HandleDelegate;
    }
    public override string HandleDelegate(List<string?> args,Session session)
    {
        StringBuilder tree = new StringBuilder();

        if(session.RootDir != null)
        {
            SendMessage(session, DrawFileTree(session.RootDir,tree, 0, new List<int>()).ToString());
            return DrawFileTree(session.RootDir,tree, 0, new List<int>()).ToString();
        }
        session.ChangeKeyboard(null);
        return tree.ToString();
    }

    private StringBuilder DrawFileTree(Directory dir, StringBuilder tree, int lvl, List<int> downLineNums)
    {
        lvl++;
        foreach(int dm in downLineNums)
            Console.WriteLine($"lvl {lvl} dln {dm} ");
        tree.AppendLine($"|__📁{dir.Name}");
        foreach(Directory d in dir.ChildDirectories)
        {
            for(int i = 0 ; i<lvl ; i++)
            {
                if(downLineNums.Exists(x => x == i))
                {
                    tree.Append("|   ");
                }
                else
                {
                    tree.Append("   ");
                }
            }

            if(d != dir.ChildDirectories.Last())
            {
                downLineNums.Add(lvl);
            }
            else
            {
                downLineNums.Remove(lvl);
            }
            DrawFileTree(d,tree,lvl,downLineNums);
            
        }

        foreach(Document d in dir.DocContents)
        {
            for(int i = 0 ; i<lvl ; i++)
            {
                if(downLineNums.Exists(x => x == i))
                {
                    tree.Append("|   ");
                }
                else
                {
                    tree.Append("   ");
                }
            }
            tree.AppendLine($"|__📄{d.Name}");

        }
        
        
        return tree;
    }

}