namespace CryptoApp;

public class BlockCryptoExecutor : CryptoExecutor
{
    
    protected override void EncryptText()
    {
        var inputFile = Terminal.GetValue("Путь до исходного файла:");
        var outputFile = Terminal.GetValue("Куда сохранить закодированный файл:");

        var key = Terminal.GetValue("Введите ключ(16 байт)");
        
        IDEA.CryptFile(inputFile, outputFile, key, true);
        
        Terminal.Message($"Файл успешно ${inputFile} зашифрован.");
    }


    protected override void DecryptText()
    {
        var inputFile = Terminal.GetValue("Путь до закодированного файла:");
        var outputFile = Terminal.GetValue("Куда сохранить раскодированный файл:");

        var key = Terminal.GetValue("Введите ключ(16 байт)");
        
        IDEA.CryptFile(inputFile, outputFile, key, false);
   
        Terminal.Message($"Файл ${inputFile} успешно расшифрован.");
    }
}