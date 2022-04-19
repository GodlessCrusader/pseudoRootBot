using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
namespace PseudoRoot
{
    class AliesTranslation
    {
        public static RootMember? TranslateKeyboardArg(string input, Directory current)
        {
            string actualName = input;
            RootMember arg = null;
            if(actualName.Contains("📁"))
            {
                actualName = actualName.Replace("📁", "");
            }

            if(actualName.Contains("📄"))
            {
                actualName = actualName.Replace("📄", "");
            }
            if(current.ChildDirectories.Exists(x => x.Name == actualName))
                    arg = current.ChildDirectories.Find(x => x.Name == actualName);
            else if(current.DocContents.Exists(x => x.Name == actualName))
                arg = current.DocContents.Find(x => x.Name == actualName);
            else
                throw new Exception("This directory doesn't exist");
            
            return arg;
        }
        public static string TranslateKeyboardCommand(string cmdLn, Session session, List<Command> commandList)
        {
            string bashCmd = cmdLn;
            if(cmdLn.Contains("📁"))
            {
                bashCmd = $"cd !{cmdLn.Replace("📁", "")}!";
                Console.WriteLine($"bashcmd through translate metod:{bashCmd}");
            }

            if(cmdLn.Contains("📄"))
            {
                bashCmd = $"get !{cmdLn.Replace("📄", "")}!";
            }

            if(cmdLn.Contains("⤴️"))
            {
                bashCmd = $"cd !{cmdLn.Replace("⤴️", "")}!";
            }

            if(cmdLn.Contains("❌"))
            {
                bashCmd = $"close";
            }
            
            else if(commandList.Exists(x => x.UserButtonAlias == cmdLn))
            {
                if(!commandList.Find(x => x.UserButtonAlias == cmdLn).RequiresArgument)
                {
                    Console.WriteLine("Is alrighth");
                    return commandList.Find(x => x.UserButtonAlias == cmdLn).Name;
                }
                else
                {
                    session.IsPerforming = commandList.Find(x => x.UserButtonAlias == cmdLn);
                   
                    if(session.IsPerforming != null)
                    {
                        if(session.IsPerforming.Name != "mkdir")
                        {    
                            session.ChangeKeyboard(null);
                            session.BotClient.SendTextMessageAsync(session.ChatId,session.IsPerforming.PerformingMessage,replyMarkup:session.Keyboard).Wait();
                        }
                        else
                        {
                            session.BotClient.SendTextMessageAsync(session.ChatId,session.IsPerforming.PerformingMessage,replyMarkup:new ReplyKeyboardRemove()).Wait();        
                        }
                    }
                    return null;
                }
            }
            return bashCmd;
        }

        public static List<string> ParseCommandLine(string cmdLn)
        {
            List<string>cmds = new List<string>();
            bool spacesCountFlag = false;
            string arg = "";
            int i = 0;
            foreach(char c in cmdLn)
            { 
                if(c == '!')
                {
                    if(spacesCountFlag)
                    {
                        cmds.Add(arg);
                        arg = "";
                        spacesCountFlag = false;
                    }
                    else
                        spacesCountFlag = true;
                }
                else
                {
                    if(spacesCountFlag)
                        arg += c;
                    else if(c == ' ')
                    {
                        cmds.Add(arg);
                        arg = "";
                    }
                    else
                    {
                        arg += c;
                        if(i == cmdLn.Length-1)
                        {
                            cmds.Add(arg);
                            arg = "";
                        }
                    }
                            
                }
                i++;

            }
                // List<int> markupPostitions = new List<int>();
                // for(int i = 0; i < cmds.Count; i++ )
                // {
                //     var check = cmds[i].Contains("!");
                //     if(check)
                //     {   
                //         markupPostitions.Add(i);
                //     }
                // }
                // string arg = "";
                // foreach(int num in markupPostitions)
                // {
                //     arg+=$"{cmds[num].Replace("!","")} ";
                // }
                // cmds[1] = arg;
                // Console.WriteLine(cmds[1]);
            
            return cmds;
        }
    }
}