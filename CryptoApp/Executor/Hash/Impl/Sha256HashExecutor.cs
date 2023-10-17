using CryptoApp.Hash;

namespace CryptoApp.Executor.Impl;

public class Sha256HashExecutor : HashExecutor
{
    public Sha256HashExecutor() : base(new Sha256())
    {
    }
}