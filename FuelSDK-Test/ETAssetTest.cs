using System;
using NUnit.Framework;

namespace FuelSDK.Test
{
    [TestFixture]
    public class ETAssetTest
    {
        ETClient client;
        ETAsset asset;

        [OneTimeSetUp]
        public void Setup()
        {
            client = new ETClient();
        }

        [SetUp]
        public void AssetSetup()
        {
            var assetObj = new ETAsset
            {
                AuthStub = client,
                Name = "Asset created using C# automated test case " + Guid.NewGuid().ToString(),
                AssetType = new AssetType { Id = 207, Name = "templatebasedemail" }
            };

            var result = assetObj.Post();
            asset = (ETAsset)result.Results[0].Object;
        }

        [TearDown]
        public void AssetTearDown()
        {
            if (asset != null)
            {
                var assetObj = new ETAsset();
                assetObj.ID = asset.ID;
                assetObj.AuthStub = client;
                assetObj.Delete();
            }
        }

        [Test]
        public void CreateAsset()
        {
            Assert.AreNotEqual(asset, null);
        }

        [Test]
        public void GetAsset()
        {
            var assetObj = new ETAsset();
            assetObj.ID = asset.ID;
            assetObj.AuthStub = client;

            var result = assetObj.Get();

            var getAssetObj = (ETAsset)result.Results[0];
            Assert.AreEqual(getAssetObj.Name, asset.Name);
            Assert.AreEqual(getAssetObj.AssetType.Id, asset.AssetType.Id);
            Assert.AreEqual(getAssetObj.AssetType.Name, asset.AssetType.Name);
        }

        [Test]
        public void DeleteAsset()
        {
            var assetObj = new ETAsset();
            assetObj.ID = asset.ID;
            assetObj.AuthStub = client;
            var result = assetObj.Delete();

            Assert.AreEqual(result.Code, 200);
            var getresult = assetObj.Get();

            Assert.AreEqual(404, getresult.Code);
            asset = null;
        }
    }
}
