namespace CryptoApp;

public interface ICipher
{
    public Stream Encrypt(Stream data);

    public Stream Decrypt(Stream data);
}