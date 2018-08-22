using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace FuelSDK
{
    class AppConfigFileConfigurationProvider : IConfigurationProvider
    {
        private readonly FuelSDKConfigurationSection customConfigSection;
        private string appSignature;
        private string autheticationEndPoint;
        private string clientId;
        private string clientSecret;
        private string restEndPoint;
        private string soapEndPoint;

        public AppConfigFileConfigurationProvider()
        {
            customConfigSection = (FuelSDKConfigurationSection)ConfigurationManager.GetSection("fuelSDK");
        }
        public string AppSignature
        {
            get { return appSignature; }
            set { appSignature = value; }
        }

        public string AuthenticationEndPoint
        {
            get { return autheticationEndPoint; }
            set { autheticationEndPoint = value; }
        }
        public string ClientId => customConfigSection.ClientId;

        public string ClientSecret => customConfigSection.ClientSecret;

        public string RestEndPoint => customConfigSection.RestEndPoint;

        public string SoapEndPoint => customConfigSection.SoapEndPoint;
    }
}
