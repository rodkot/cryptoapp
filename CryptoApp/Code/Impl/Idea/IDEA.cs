namespace CryptoApp;

public class IDEA
{
    // Number of rounds.
    private const int Rounds = 8;
    private const int BlockSize = 8;

    // Internal encryption sub-keys.
    private readonly int[] _subKey;

    public IDEA(string charKey, bool encrypt)
    {
        byte[] key = GenerateUserKeyFromCharKey(charKey);
        // Expands a 16-byte user key to the internal encryption sub-keys.
        int[] tempSubKey = ExpandUserKey(key);
        if (encrypt)
        {
            _subKey = tempSubKey;
        }
        else
        {
            _subKey = InvertSubKey(tempSubKey);
        }
    }

    /**
     * Encrypts or decrypts a block of 8 data bytes.
     */
    public void Crypt(byte[] data)
    {
        Сrypt(data, 0);
    }

    /**
     * Encrypts or decrypts a block of 8 data bytes.
     */
    public void Сrypt(byte[] data, int dataPos)
    {
        int x0 = ((data[dataPos + 0] & 0xFF) << 8) | (data[dataPos + 1] & 0xFF);
        int x1 = ((data[dataPos + 2] & 0xFF) << 8) | (data[dataPos + 3] & 0xFF);
        int x2 = ((data[dataPos + 4] & 0xFF) << 8) | (data[dataPos + 5] & 0xFF);
        int x3 = ((data[dataPos + 6] & 0xFF) << 8) | (data[dataPos + 7] & 0xFF);
        //
        int p = 0;
        for (int round = 0; round < Rounds; round++)
        {
            int y0 = Mul(x0, _subKey[p++]);
            int y1 = Add(x1, _subKey[p++]);
            int y2 = Add(x2, _subKey[p++]);
            int y3 = Mul(x3, _subKey[p++]);
            //
            int t0 = Mul(y0 ^ y2, _subKey[p++]);
            int t1 = Add(y1 ^ y3, t0);
            int t2 = Mul(t1, _subKey[p++]);
            int t3 = Add(t0, t2);
            //
            x0 = y0 ^ t2;
            x1 = y2 ^ t2;
            x2 = y1 ^ t3;
            x3 = y3 ^ t3;
        }

        //
        int r0 = Mul(x0, _subKey[p++]);
        int r1 = Add(x2, _subKey[p++]);
        int r2 = Add(x1, _subKey[p++]);
        int r3 = Mul(x3, _subKey[p++]);
        //
        data[dataPos + 0] = (byte)(r0 >> 8);
        data[dataPos + 1] = (byte)r0;
        data[dataPos + 2] = (byte)(r1 >> 8);
        data[dataPos + 3] = (byte)r1;
        data[dataPos + 4] = (byte)(r2 >> 8);
        data[dataPos + 5] = (byte)r2;
        data[dataPos + 6] = (byte)(r3 >> 8);
        data[dataPos + 7] = (byte)r3;
    }

    // Expands a 16-byte user key to the internal encryption sub-keys.
    private int[] ExpandUserKey(byte[] userKey)
    {
        if (userKey.Length != 16)
        {
            throw new ArgumentException("Key length must be 128 bit", "key");
        }

        int[] key = new int[Rounds * 6 + 4];
        for (int i = 0; i < userKey.Length / 2; i++)
        {
            key[i] = ((userKey[2 * i] & 0xFF) << 8) | (userKey[2 * i + 1] & 0xFF);
        }

        for (int i = userKey.Length / 2; i < key.Length; i++)
        {
            key[i] = ((key[(i + 1) % 8 != 0 ? i - 7 : i - 15] << 9) | (key[(i + 2) % 8 < 2 ? i - 14 : i - 6] >> 7)) &
                     0xFFFF;
        }

        return key;
    }

    public static void CryptFile(String inputFileName, String outputFileName, String charKey, bool encrypt)
    {
        FileStream inStream = null;
        FileStream outStream = null;
        try
        {
            IDEA idea = new IDEA(charKey, encrypt);
            BlockStreamCrypter bsc = new BlockStreamCrypter(idea, encrypt);
            inStream = new FileStream(inputFileName, FileMode.Open, FileAccess.ReadWrite);
            long inFileSize = inStream.Length;
            long inDataLen;
            long outDataLen;
            if (encrypt)
            {
                inDataLen = inFileSize;
                outDataLen = (inDataLen + BlockSize - 1) / BlockSize * BlockSize;
            }
            else
            {
                if (inFileSize == 0)
                {
                    throw new IOException("Input file is empty.");
                }

                if (inFileSize % BlockSize != 0)
                {
                    throw new IOException("Input file size is not a multiple of " + BlockSize + ".");
                }

                inDataLen = inFileSize - BlockSize;
                outDataLen = inDataLen;
            }

            outStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
            ProcessData(inStream, inDataLen, outStream, outDataLen, bsc);
            if (encrypt)
            {
                WriteDataLength(outStream, inDataLen, bsc);
            }
            else
            {
                long outFileSize = ReadDataLength(inStream, bsc);
                if (outFileSize < 0 || outFileSize > inDataLen || outFileSize < inDataLen - BlockSize + 1)
                {
                    throw new IOException("Input file is not a valid cryptogram.");
                }

                if (outFileSize != outDataLen)
                {
                    outStream.SetLength(outFileSize);
                }
            }

            outStream.Close();
        }
        finally
        {
            if (inStream != null)
            {
                inStream.Close();
            }

            if (outStream != null)
            {
                outStream.Close();
            }
        }
    }

    private class BlockStreamCrypter
    {
        IDEA idea;

        bool encrypt;

        // data of the previous ciphertext block
        byte[] prev;
        byte[] newPrev;

        public BlockStreamCrypter(IDEA idea, bool encrypt)
        {
            this.idea = idea;
            this.encrypt = encrypt;
            prev = new byte[BlockSize];
            newPrev = new byte[BlockSize];
        }

        public void Сrypt(byte[] data, int pos)
        {
            if (encrypt)
            {
                Xor(data, pos, prev);
                idea.Сrypt(data, pos);
                Array.Copy(data, pos, prev, 0, BlockSize);
            }
            else
            {
                Array.Copy(data, pos, newPrev, 0, BlockSize);
                idea.Сrypt(data, pos);
                Xor(data, pos, prev);
                (prev, newPrev) = (newPrev, prev);
            }
        }
    }

    private static void ProcessData(Stream inStream, long inDataLen, Stream outStream, long outDataLen,
        BlockStreamCrypter bsc)
    {
        int bufSize = 0x200000;
        byte[] buf = new byte[bufSize];
        long filePos = 0;
        while (filePos < inDataLen)
        {
            int reqLen = (int)Math.Min(inDataLen - filePos, bufSize);
            int trLen = inStream.Read(buf, 0, reqLen);
            if (trLen != reqLen)
            {
                throw new Exception("Incomplete data chunk read from file.");
            }

            int chunkLen = (trLen + BlockSize - 1) / BlockSize * BlockSize;
            for (int i = trLen; i <= chunkLen; i++)
            {
                buf[i] = 0;
            }

            for (int pos = 0; pos < chunkLen; pos += BlockSize)
            {
                bsc.Сrypt(buf, pos);
            }

            reqLen = (int)Math.Min(outDataLen - filePos, chunkLen);

            outStream.Write(buf, 0, reqLen);

            filePos += chunkLen;
        }
    }

    private static void Xor(byte[] a, int pos, byte[] b)
    {
        for (int p = 0; p < BlockSize; p++)
        {
            a[pos + p] ^= b[p];
        }
    }

    private static long ReadDataLength(FileStream stream, BlockStreamCrypter bsc)
    {
        byte[] buf = new byte[BlockSize];
        int trLen = stream.Read(buf, 0, BlockSize);
        if (trLen != BlockSize)
        {
            throw new Exception("Unable to read data length suffix.");
        }

        bsc.Сrypt(buf, 0);
        return UnpackDataLength(buf);
    }

    private static void WriteDataLength(FileStream stream, long dataLength, BlockStreamCrypter bsc)
    {
        byte[] a = PackDataLength(dataLength);
        bsc.Сrypt(a, 0);
        stream.Write(a, 0, BlockSize);
    }

    // Packs an integer into an 8-byte block. Used to encode the file size.
    private static byte[] PackDataLength(long i)
    {
        if (i > 0x1FFFFFFFFFFFL) // 45 bits
        {
            throw new ArgumentException("Text too long.");
        }

        byte[] b = new byte[BlockSize];
        b[7] = (byte)(i << 3);
        b[6] = (byte)(i >> 5);
        b[5] = (byte)(i >> 13);
        b[4] = (byte)(i >> 21);
        b[3] = (byte)(i >> 29);
        b[2] = (byte)(i >> 37);
        return b;
    }

    // Extracts an integer from an 8-byte block. Used to decode the file size.
    private static long UnpackDataLength(byte[] b)
    {
        if (b[0] != 0 || b[1] != 0 || (b[7] & 7) != 0)
        {
            return -1;
        }

        return
            (long)(b[7] & 0xFF) >> 3 |
            (long)(b[6] & 0xFF) << 5 |
            (long)(b[5] & 0xFF) << 13 |
            (long)(b[4] & 0xFF) << 21 |
            (long)(b[3] & 0xFF) << 29 |
            (long)(b[2] & 0xFF) << 37;
    }

    // Inverts decryption/encrytion sub-keys to encrytion/decryption sub-keys.
    private static int[] InvertSubKey(int[] key)
    {
        int[] invKey = new int[key.Length];
        int p = 0;
        int i = Rounds * 6;
        invKey[i + 0] = MulInv(key[p++]);
        invKey[i + 1] = AddInv(key[p++]);
        invKey[i + 2] = AddInv(key[p++]);
        invKey[i + 3] = MulInv(key[p++]);
        for (int r = Rounds - 1; r >= 0; r--)
        {
            i = r * 6;
            int m = r > 0 ? 2 : 1;
            int n = r > 0 ? 1 : 2;
            invKey[i + 4] = key[p++];
            invKey[i + 5] = key[p++];
            invKey[i + 0] = MulInv(key[p++]);
            invKey[i + m] = AddInv(key[p++]);
            invKey[i + n] = AddInv(key[p++]);
            invKey[i + 3] = MulInv(key[p++]);
        }

        return invKey;
    }

    // Addition in the additive group.
    private static int Add(int a, int b)
    {
        return (a + b) & 0xFFFF;
    }

    // Multiplication in the multiplicative group.
    private static int Mul(int a, int b)
    {
        long r = (long)a * b;
        if (r != 0)
        {
            return (int)(r % 0x10001) & 0xFFFF;
        }
        else
        {
            return (1 - a - b) & 0xFFFF;
        }
    }

    // Additive Inverse.
    private static int AddInv(int x)
    {
        return (0x10000 - x) & 0xFFFF;
    }

    // Multiplicative inverse.
    private static int MulInv(int x)
    {
        if (x <= 1)
        {
            return x;
        }

        int y = 0x10001;
        int t0 = 1;
        int t1 = 0;
        while (true)
        {
            t1 += y / x * t0;
            y %= x;
            if (y == 1)
            {
                return 0x10001 - t1;
            }

            t0 += x / y * t1;
            x %= y;
            if (x == 1)
            {
                return t0;
            }
        }
    }

    // Generates a 16-byte binary user key from a character string key.
    private static byte[] GenerateUserKeyFromCharKey(String charKey)
    {
        int nofChar = 0x7E - 0x21 + 1; // Number of different valid characters
        int[] a = new int[8];
        for (int p = 0; p < charKey.Length; p++)
        {
            int c = charKey[p];

            for (int i = a.Length - 1; i >= 0; i--)
            {
                c += a[i] * nofChar;
                a[i] = c & 0xFFFF;
                c >>= 16;
            }
        }

        byte[] key = new byte[16];
        for (int i = 0; i < 8; i++)
        {
            key[i * 2] = (byte)(a[i] >> 8);
            key[i * 2 + 1] = (byte)a[i];
        }

        return key;
    }
}