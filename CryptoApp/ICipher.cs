namespace CryptoApp;

public interface ICipher
{
    public byte[] Encrypt(byte[] data);

    public byte[] Decrypt(byte[] data);
}