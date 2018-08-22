﻿using NUnit.Framework;
using System;
using FuelSDK;
using Newtonsoft.Json.Linq;

namespace FuelSDK.Test
{
    [TestFixture()]
    public class ETCampaignTest
    {

        ETClient client;
        ETCampaign campaign;
        string campaignName;
        string campaignDesc;


        [OneTimeSetUp]
        public void Setup()
        {
            try {
                client = new ETClient();
            }
            catch(System.Net.WebException ex) {
                var a = ex;
                throw;
            }
        }

        [SetUp] 
        public void CampaignSetup()
        {
			campaignName = Guid.NewGuid().ToString();
			campaignDesc = "Campaign created using C# automated test case";

			var campObj = new ETCampaign(client)
			{
				Name = campaignName,
				Description = campaignDesc
			};

			var result = campObj.Post();
			campaign = (ETCampaign)result.Results[0].Object;
        }

        [TearDown]
        public void CampaignTearDown()
        {
            if(campaign != null)
            {
				var campObj = new ETCampaign(client);
				campObj.ID = campaign.ID;
                campObj.Delete();
            }
        }

        [Test()]
        public void CreateCampaign()
        {
            Assert.AreNotEqual(campaign, null);
        }

        [Test()]
        public void GetCampaign()
        {
            var campObj = new ETCampaign(client);
            campObj.ID = campaign.ID;
            var result = campObj.Get();
            var getCampObj = (ETCampaign) result.Results[0];
            Assert.AreEqual(getCampObj.Name,campaignName);
            Assert.AreEqual(getCampObj.Description, campaignDesc);
        }

        [Test()]
        public void DeleteCampaign()
        {
			var campObj = new ETCampaign(client);
			campObj.ID = campaign.ID;
            var result = campObj.Delete();

            Assert.AreEqual(result.Code,200);

            var getresult = campObj.Get();

            JToken message = JObject.Parse(getresult.Message);
            var msg = message.SelectToken("message").ToString();

            Assert.AreEqual(msg,"Campaign does not exist");
            campaign = null;
        }
    }


}
