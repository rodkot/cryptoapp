namespace CryptoApp;

class RC4 : Code
{
    private readonly byte[] _s;

    public RC4(byte[] key)
    {
        _s = new byte[256];
        var t = new byte[256];

        for (int i = 0; i < 256; i++)
        {
            _s[i] = (byte)i;
            t[i] = key[i % key.Length];
        }

        int j = 0;
        for (int i = 0; i < 256; i++)
        {
            j = (j + _s[i] + t[i]) % 256;
            (_s[i], _s[j]) = (_s[j], _s[i]);
        }
    }

    private Stream Crypt(Stream data)
    {
        MemoryStream output = new MemoryStream();

        using (MemoryStream input = new MemoryStream())
        {
            data.CopyTo(input);
            input.Position = 0;
            int i = 0;
            int j = 0;
            for (int x = 0; x < input.Length; x++)
            {
                var current = (byte)input.ReadByte();

                i = (i + 1) % 256;
                j = (j + _s[i]) % 256;

                (_s[i], _s[j]) = (_s[j], _s[i]);

                byte k = _s[(_s[i] + _s[j]) % 256];
                output.WriteByte((byte)(current ^ k));
            }
        }

        output.Position = 0;
        return output;
    }

    public override Stream Encrypt(Stream data)
    {
        return Crypt(data);
    }

    public override Stream Decrypt(Stream data)
    {
        return Crypt(data);
    }
}