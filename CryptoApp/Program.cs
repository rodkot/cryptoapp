// See https://aka.ms/new-console-template for more information


namespace CryptoApp;

class Program
{
    private static readonly BlockCrypto BlockCrypto = new BlockCrypto();
    private static readonly StreamCrypto StreamCrypto = new StreamCrypto();

    static void Main(string[] args)
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Terminal.Message("________________________________________________");
            Terminal.Message("Выберите шифр:");
            Terminal.Message("1. Потовый шифр");
            Terminal.Message("2. Блочный шифр");
            Terminal.Message("3. Выйти");
            string choice = Console.ReadLine();
            Console.Clear();
            switch (choice)
            {
                case "1":
                    StreamCrypto.Exec();
                    break;
                case "2":
                    BlockCrypto.Exec();
                    break;
                case "3":
                    exit = true;
                    break;
                default:
                    Terminal.Message("Неверный выбор. Попробуйте еще раз.");
                    break;
            }

            Console.ReadLine(); // Подождать, пока пользователь нажмет Enter перед возвратом к меню.
        }
    }
}