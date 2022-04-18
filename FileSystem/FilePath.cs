class FilePath
{
    public List<string> directories = new List<string>();

    public string GetString()
    {
        string path = @"$ \rom";
        foreach(string s in directories)
        {
            path = path + @"\" + s;
        }
        return path;
    }
}