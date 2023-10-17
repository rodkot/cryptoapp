namespace CryptoApp;

public abstract class Code: ICipher
{
    protected byte[] _key;

    public Code(byte[] key)
    {
        _key = key;
    }

    public abstract byte[] Encrypt(byte[] data);


    public abstract byte[] Decrypt(byte[] data);
}