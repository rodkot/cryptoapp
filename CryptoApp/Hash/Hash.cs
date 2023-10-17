using System.Collections.ObjectModel;

namespace CryptoApp.Hash;

public abstract class Hash
{
    public abstract ReadOnlyCollection<byte> Calculate(Stream stream);
}