namespace CryptoApp;

public static class Terminal
{
    public static string GetValue(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine();
    }

    public static void Message(string message)
    {
        Console.WriteLine(message);
    }
}