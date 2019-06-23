using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FuelSDK
{
    class ConfigFileWithParametersOverwriteConfigurationProvider : IConfigurationProvider
    {
        private IFuelSDKConfiguration configuration;
        
        public IFuelSDKConfiguration Get(NameValueCollection parameters)
        {
            configuration = new FuelSDKConfiguration();

            if (parameters != null)
            {
                if (parameters.AllKeys.Contains("appSignature"))
                    configuration.AppSignature = parameters["appSignature"];
                if (parameters.AllKeys.Contains("clientId"))
                    configuration.ClientId = parameters["clientId"];
                if (parameters.AllKeys.Contains("clientSecret"))
                    configuration.ClientSecret = parameters["clientSecret"];
                if (parameters.AllKeys.Contains("soapEndPoint"))
                    configuration.SoapEndPoint = parameters["soapEndPoint"];
                if (parameters.AllKeys.Contains("authEndPoint"))
                {
                    configuration.AuthenticationEndPoint = parameters["authEndPoint"];
                }
                if (parameters.AllKeys.Contains("restEndPoint"))
                {
                    configuration.RestEndPoint = parameters["restEndPoint"];
                }
                if (parameters.AllKeys.Contains("useOAuth2Authentication"))
                {
                    configuration.UseOAuth2Authentication = parameters["useOAuth2Authentication"];
                }
                if (parameters.AllKeys.Contains("accountId"))
                {
                    configuration.AccountId = parameters["accountId"];
                }
                if (parameters.AllKeys.Contains("scope"))
                {
                    configuration.Scope = parameters["scope"];
                }
            }
            if (string.IsNullOrEmpty(configuration.ClientId) || string.IsNullOrEmpty(configuration.ClientSecret))
                throw new Exception("clientId or clientSecret is null: Must be provided in config file or passed when instantiating ETClient");

            return configuration;
        }
    }
}
