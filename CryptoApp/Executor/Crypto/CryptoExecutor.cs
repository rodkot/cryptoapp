using System.Text;
using CryptoApp.Executor;

namespace CryptoApp;

public abstract class CryptoExecutor : IExecutor
{
    public void Exec() 
    {  
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Terminal.Message("________________________________________________");
            Terminal.Message("Выберите действие:");
            Terminal.Message("1. Зашифровать");
            Terminal.Message("2. Расшифровать");
            Terminal.Message("3. Выйти");
            string choice = Console.ReadLine();
            Console.Clear();
            switch (choice)
            {
                case "1":
                    EncryptText();
                    break;
                case "2":
                    DecryptText();
                    break;
                case "3":
                   return;
                default:
                    Terminal.Message("Неверный выбор. Попробуйте еще раз.");
                    break;
            }
            Console.ReadLine(); // Подождать, пока пользователь нажмет Enter перед возвратом к меню.
        }
        
    }

    protected byte[] InputKey()
    {
        var key = Terminal.GetValue("Введите секретный ключ (например, 'MySecretKey'):");
        return Encoding.ASCII.GetBytes(key);
    }

    protected abstract void EncryptText();
    protected abstract void DecryptText();
}