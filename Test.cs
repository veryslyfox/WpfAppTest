using System.IO;
static class Test
{

    public static StreamWriter File;
    public static void InitTest()
    {
        File = new(System.IO.File.Open("log.txt", FileMode.OpenOrCreate));
    }
    public static void Log(string message)
    {
        File.WriteLine(message);
    }
}
