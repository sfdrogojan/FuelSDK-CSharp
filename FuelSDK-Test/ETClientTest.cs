using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FuelSDK.Test
{
    [TestFixture()]
    class ETClientTest
    {
        ETClient client1;
        ETClient client2;

        [OneTimeSetUp]
        public void Setup()
        {
            client1 = new ETClient();
            client2 = new ETClient();
        }

        [Test()]
        public void GetClientStack()
        {
            Assert.IsNotNull(client1.Stack);
        }

        [Test()]
        public void TestSoapEndpointCaching()
        {
            var client1SoapEndpointExpirationField = client1.GetType().GetField("soapEndpointExpiration", BindingFlags.NonPublic | BindingFlags.Static);
            var client2SoapEndpointExpirationField = client2.GetType().GetField("soapEndpointExpiration", BindingFlags.NonPublic | BindingFlags.Static);

            var client1SoapEndpointExpiration = (long)client1SoapEndpointExpirationField.GetValue(null);
            var client2SoapEndpointExpiration = (long)client2SoapEndpointExpirationField.GetValue(null);

            Assert.IsTrue(client1SoapEndpointExpiration > 0);
            Assert.IsTrue(client2SoapEndpointExpiration > 0);
            Assert.AreEqual(client1SoapEndpointExpiration, client2SoapEndpointExpiration);
        }
    }
}
