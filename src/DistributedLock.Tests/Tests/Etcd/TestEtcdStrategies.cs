#if NET8_0_OR_GREATER
using Medallion.Threading.Etcd;
using Medallion.Threading.Etcd.Tests;
using NUnit.Framework;

namespace Medallion.Threading.Tests.Etcd;

public sealed class
    TestingEtcdLeaseDistributedLockProvider : TestingLockProvider<TestingEtcdLeaseSynchronizationStrategy>
{
    private readonly EtcdClusterSetup _etcdClusterSetup;

    public TestingEtcdLeaseDistributedLockProvider()
    {
        this._etcdClusterSetup = new EtcdClusterSetup();
        this._etcdClusterSetup.ClusterSetup();
    }

    public override void Dispose()
    {
        
    }
    
    
    public override IDistributedLock CreateLockWithExactName(string name)
    {
        return new EtcdLeaseDistributedLock(this._etcdClusterSetup.CreateClientToEtcdCluster().Result, name,
            this.Strategy.Options);
    }

    public override string GetSafeName(string name) =>
        EtcdLeaseDistributedLock.GetSafeName(name);
}
public sealed class TestingEtcdLeaseSynchronizationStrategy : TestingSynchronizationStrategy
{
    private readonly DisposableCollection _disposables = new();

    private static readonly Action<EtcdLeaseOptionsBuilder> DefaultTestingOptions = o =>
        // for test speed
        o.BusyWaitSleepTime(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(25));


    public Action<EtcdLeaseOptionsBuilder>? Options { get; set; } = DefaultTestingOptions;
    public bool CreateBlobBeforeLockIsCreated { get; set; }

    public override IDisposable? PrepareForHandleLost()
    {
        this.Options = o =>
        {
            DefaultTestingOptions(o);
            o.RenewalCadence(TimeSpan.FromMilliseconds(10));
        };

        return new HandleLostScope();
    }

    public override void PrepareForHighContention(ref int maxConcurrentAcquires)
    {
        this.Options = null; // reduces # of requests under high contention
        this.CreateBlobBeforeLockIsCreated = true;
    }

    public override void Dispose()
    {
        try { this._disposables.Dispose(); }
        finally { base.Dispose(); }
    }

    private class HandleLostScope : IDisposable
    {
        public void Dispose()
        {
            // TODO: ???
        }
    }
}
#endif