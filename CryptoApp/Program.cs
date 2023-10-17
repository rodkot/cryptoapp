
using CryptoApp.Executor.Impl;

namespace CryptoApp;

class Program
{
    private static readonly BlockCryptoExecutor BlockCryptoExecutor = new();
    private static readonly StreamCryptoExecutor StreamCryptoExecutor = new();
    private static readonly Sha256HashExecutor Sha256HashExecutor = new();
    

    static void Main(string[] args)
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Terminal.Message("________________________________________________");
            Terminal.Message("Выберите режим:");
            Terminal.Message("1. Потовое шифрование");
            Terminal.Message("2. Блочное шифрование");
            Terminal.Message("3. Подчет SHA 256");
            Terminal.Message("4. Выйти");
            string choice = Console.ReadLine();
            Console.Clear();
            switch (choice)
            {
                case "1":
                    StreamCryptoExecutor.Exec();
                    break;
                case "2":
                    BlockCryptoExecutor.Exec();
                    break;
                case "3":
                    Sha256HashExecutor.Exec();
                    break;
                case "4":
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