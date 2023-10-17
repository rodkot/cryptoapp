using System.Text;

namespace CryptoApp;

public class StreamCryptoExecutor : CryptoExecutor
{
    protected override void EncryptText()
    {
        Console.WriteLine("Введите строку для шифрования:");
        string inputText = Console.ReadLine();
        Console.WriteLine("Введите секретный ключ (например, 'MySecretKey'):");
        string key = Console.ReadLine();

        RC4 rc4 = new RC4(Encoding.ASCII.GetBytes(key));
        Stream encrypted = rc4.Encrypt(new MemoryStream(Encoding.ASCII.GetBytes(inputText)));
        
        using (MemoryStream stream = new MemoryStream())
        {
            encrypted.CopyTo(stream);
            
            string hexString = BitConverter.ToString(stream.ToArray()).Replace("-", "");
            
            Console.WriteLine("Зашифрованное сообщение: " + hexString);
        } 
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

        RC4 rc4 = new RC4(key);
        Stream decrypted = rc4.Decrypt(new MemoryStream(encryptedBytes));
        decrypted.Position = 0;
        StreamReader reader = new StreamReader(decrypted, Encoding.Default);
        string content = reader.ReadToEnd();
        Terminal.Message($"Расшифрованное сообщение: {content}");
    }
}