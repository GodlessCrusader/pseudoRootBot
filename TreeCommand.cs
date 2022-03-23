
using System.Text;
using Telegram.Bot;

class TreeCommand : Command {

    public TreeCommand(TelegramBotClient botClient, CancellationToken cts)
    {
        this.Name = "tree";
        this.BotClient = botClient;
        this.CancellationToken = cts;
    }
    public override string Handle(string cmdLn, FilePath pwd, string fileName, long chatId)
    {
        StringBuilder tree = new StringBuilder();

        Directory? rootDir = null;

        using(StreamReader sr = new StreamReader(fileName))
        {
            rootDir=Newtonsoft.Json.JsonConvert.DeserializeObject<Directory>(sr.ReadToEnd());
        }

        if(rootDir != null)
        {
           return DrawFileTree(rootDir,tree, 0, new List<int>()).ToString();
        }
        
        return tree.ToString();
    }

    private StringBuilder DrawFileTree(Directory dir, StringBuilder tree, int lvl, List<int> downLineNums)
    {
        lvl++;
        foreach(int dm in downLineNums)
            Console.WriteLine($"lvl {lvl} dln {dm} ");
        tree.AppendLine($"|__{dir.Name}");
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
            tree.AppendLine($"|__{d.Name}");

        }
        
        
        return tree;
    }

}