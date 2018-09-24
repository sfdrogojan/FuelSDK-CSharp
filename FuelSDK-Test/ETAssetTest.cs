using NUnit.Framework;

namespace FuelSDK.Test
{
    [TestFixture()]
    public class ETAssetTest
    {
        ETClient client;
        ETAsset asset;

        [OneTimeSetUp]
        public void Setup()
        {
            client = new ETClient();

            var assetObj = new ETAsset
            {
                AuthStub = client,
                Name = "NTO Welcome Series Email",
                AssetType = new AssetType { Id = 207, Name = "templatebasedemail" }
            };

            var result = assetObj.Post();
            asset = (ETAsset)result.Results[0].Object;
        }

        [Test()]
        public void CreateAsset()
        {
            Assert.AreNotEqual(asset, null);
        }
    }
}
