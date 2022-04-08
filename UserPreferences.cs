class UserPreferences
{
    public CommandMode PreferedMode;
    public Directory RootDir;

    public string Version;



}

enum CommandMode
{
    BashConsole = 0,
    InternalKeyboard = 1

}