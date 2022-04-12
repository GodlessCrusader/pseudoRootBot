class UserPreferences
{
    public CommandMode PreferedMode = CommandMode.InternalKeyboard;
    public Directory RootDir;

    public string Version= "";

    public UserPreferences()
    {
        RootDir = new Directory("rom", null);
    }

}

enum CommandMode
{
    BashConsole = 0,
    InternalKeyboard = 1

}