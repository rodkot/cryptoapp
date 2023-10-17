namespace CryptoApp;

class RC4
{
    private byte[] _s;
    private byte[] _t;

    public RC4(byte[] key)
    {
        _s = new byte[256];
        _t = new byte[256];

        for (int i = 0; i < 256; i++)
        {
            _s[i] = (byte)i;
            _t[i] = key[i % key.Length];
        }

        int j = 0;
        for (int i = 0; i < 256; i++)
        {
            j = (j + _s[i] + _t[i]) % 256;
            (_s[i], _s[j]) = (_s[j], _s[i]);
        }
    }

    public byte[] Encrypt(byte[] input)
    {
        byte[] output = new byte[input.Length];
        int i = 0;
        int j = 0;

        for (int x = 0; x < input.Length; x++)
        {
            i = (i + 1) % 256;
            j = (j + _s[i]) % 256;

            (_s[i], _s[j]) = (_s[j], _s[i]);

            byte k = _s[(_s[i] + _s[j]) % 256];
            output[x] = (byte)(input[x] ^ k);
        }

        return output;
    }
}