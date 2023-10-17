using System.Collections.ObjectModel;
using System.Text;

namespace CryptoApp.Executor;
using Hash;
public  abstract class HashExecutor: IExecutor
{
    private Hash _hash;

    protected HashExecutor(Hash hash)
    {
        _hash = hash;
    }

    public void Exec()
    {
        var inputFile = Terminal.GetValue("Путь до исходного файла:");
        FileStream  inStream = new FileStream(inputFile, FileMode.Open, FileAccess.ReadWrite);
        var hash = _hash.Calculate(inStream);
        System.Console.Out.WriteLine("{0}", ArrayToString(hash));
    }
    
    private string ArrayToString(ReadOnlyCollection<byte> arr)
    {
        StringBuilder s = new StringBuilder(arr.Count * 2);
        for (int i = 0; i < arr.Count; ++i)
        {
            s.AppendFormat("{0:x2}", arr[i]);
        }

        return s.ToString();
    }
}