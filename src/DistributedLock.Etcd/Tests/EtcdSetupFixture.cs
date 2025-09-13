using NUnit.Framework;

namespace Medallion.Threading.Etcd.Tests;

[SetUpFixture]
public class EtcdSetupFixture
{
    public static readonly EtcdClusterSetup EtcdClusterSetup = new EtcdClusterSetup();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await EtcdClusterSetup.ClusterSetup();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await EtcdClusterSetup.TearDownCluster();
    }
}
