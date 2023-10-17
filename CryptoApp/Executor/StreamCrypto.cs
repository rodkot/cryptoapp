using System.Text;

namespace CryptoApp;

public class StreamCrypto: CryptoExecutor
{
    protected override void EncryptText()
    {
        Console.WriteLine("Введите строку для шифрования:");
        string inputText = Console.ReadLine();
        Console.WriteLine("Введите секретный ключ (например, 'MySecretKey'):");
        string key = Console.ReadLine();

        RC4 rc4 = new RC4(Encoding.ASCII.GetBytes(key));
        byte[] encryptedBytes = rc4.Encrypt(Encoding.ASCII.GetBytes(inputText));
        string encryptedText = BitConverter.ToString(encryptedBytes).Replace("-", "").ToLower();

        Console.WriteLine($"Зашифрованная строка: {encryptedText}");
    }

    protected override void DecryptText()
    {
        string inputText =
            Terminal.GetValue("Введите строку для расшифровки (в шестнадцатеричном формате, без дефисов):");

        byte[] key = InputKey();

        // Преобразование шестнадцатеричной строки в массив байтов
        byte[] encryptedBytes = new byte[inputText.Length / 2];
        for (int i = 0; i < encryptedBytes.Length; i++)
        {
            encryptedBytes[i] = Convert.ToByte(inputText.Substring(i * 2, 2), 16);
        }

        RC4 rc4Decrypt = new RC4(key);
        byte[] decryptedBytes = rc4Decrypt.Encrypt(encryptedBytes);
        string decryptedText = Encoding.ASCII.GetString(decryptedBytes);

        Terminal.Message($"Расшифрованная строка: {decryptedText}");
        
    }
    
}