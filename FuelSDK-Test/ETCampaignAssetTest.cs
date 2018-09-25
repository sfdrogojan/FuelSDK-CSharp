using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
namespace FuelSDK.Test
{
    [TestFixture()]
    public class ETCampaignAssetTest
    {
		ETClient client;
        ETCampaignAsset campaignAsset;
        ETCampaign campaign;
        ETAsset asset;

        [OneTimeSetUp]
		public void Setup()
        {
            client = new ETClient();
        }

        [SetUp]
		public void CampaignAssetSetup()
        {
            CreateCampaign();
            CreateAsset();
            CreateCampaignAssetObject();
        }

        [Test()]
		public void CreateCampaignAsset()
		{
			Assert.AreNotEqual(campaignAsset, null);
		}

        [Test()]
        public void GetCampaignAsset()
        {
            var campaignAssetObj = new ETCampaignAsset();
            campaignAssetObj.ID = campaignAsset.ID;
            campaignAssetObj.CampaignID = campaign.ID.ToString();
            campaignAssetObj.AuthStub = client;

            var result = campaignAssetObj.Get();

            var getCampaignAssetObj = (ETCampaignAsset)result.Results[0];
            Assert.AreEqual(getCampaignAssetObj.Type, campaignAsset.Type);
        }

        [Test()]
        public void DeleteCampaignAsset()
        {
            var campAssetObj = new ETCampaignAsset();
            campAssetObj.ID = campaignAsset.ID;
            campAssetObj.CampaignID = campaignAsset.CampaignID;
            campAssetObj.AuthStub = client;
            var result = campAssetObj.Delete();

            Assert.AreEqual(200, result.Code);

            var getResult = campAssetObj.Get();

            JToken message = JObject.Parse(getResult.Message);
            var msg = message.SelectToken("message").ToString();

            Assert.AreEqual(msg, "Campaign Asset does not exist");
            campaignAsset = null;
        }

        [TearDown]
		public void CampaignAssetTearDown()
        {
            DeleteCampaignAssetObject();
            DeleteAsset();
            DeleteCampaignObject();
        }

        private void CreateCampaign()
        {
            var campObj = new ETCampaign
            {
                AuthStub = client,
                Name = "Created for testing campaign asset test cases in C#",
                Description = "Test Description"
            };

            var result = campObj.Post();
            campaign = (ETCampaign)result.Results[0].Object;
        }

        private void CreateAsset()
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

        private void CreateCampaignAssetObject()
        {
            var campaignAssetObj = new ETCampaignAsset
            {
                AuthStub = client,
                CustomerKey = Guid.NewGuid().ToString(),
                Type = "CMS_ASSET",
                CampaignID = campaign.ID.ToString(),
                IDs = new string[] { asset.ID.ToString() }
            };

            var result = campaignAssetObj.Post();
            campaignAsset = (ETCampaignAsset)result.Results[0].Object;
        }

        public void DeleteCampaignObject()
        {
            if (campaign != null)
            {
                var campObj = new ETCampaign();
                campObj.ID = campaign.ID;
                campObj.AuthStub = client;
                var result = campObj.Delete();
            }

            campaign = null;
        }

        public void DeleteAsset()
        {
            if (asset != null)
            {
                var assetObj = new ETAsset();
                assetObj.ID = asset.ID;
                assetObj.AuthStub = client;
                var result = assetObj.Delete();
            }

            asset = null;
        }

        private void DeleteCampaignAssetObject()
        {
            if (campaignAsset != null)
            {
                var campaignAssetObj = new ETCampaignAsset();
                campaignAssetObj.ID = campaignAsset.ID;
                campaignAssetObj.CampaignID = campaignAsset.CampaignID;
                campaignAssetObj.AuthStub = client;
                campaignAssetObj.Delete();
            }

            campaignAsset = null;
        }
    }
}
