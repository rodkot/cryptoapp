namespace CryptoApp;

public abstract class Code: ICipher
{ 
    public abstract Stream Encrypt(Stream data);
    public abstract Stream Decrypt(Stream data);
}