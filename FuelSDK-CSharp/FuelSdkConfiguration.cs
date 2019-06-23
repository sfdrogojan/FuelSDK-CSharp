using System;

namespace FuelSDK
{
    public class FuelSDKConfiguration : IFuelSDKConfiguration
    {
        private readonly FuelSDKConfigurationSection configSection;

        public FuelSDKConfiguration()
        {
            configSection = ConfigUtil.GetFuelSDKConfigSection();
            configSection = (configSection != null ? (FuelSDKConfigurationSection)configSection.Clone() : new FuelSDKConfigurationSection());
            configSection = configSection
                .WithDefaultAuthEndpoint(DefaultEndpoints.Auth)
                .WithDefaultRestEndpoint(DefaultEndpoints.Rest);

            AppSignature = configSection.AppSignature;
            ClientId = configSection.ClientId;
            ClientSecret = configSection.ClientSecret;
            SoapEndPoint = configSection.SoapEndPoint;
            AuthenticationEndPoint = configSection.AuthenticationEndPoint;
            RestEndPoint = configSection.RestEndPoint;
            UseOAuth2Authentication = configSection.UseOAuth2Authentication;
            AccountId = configSection.AccountId;
            Scope = configSection.Scope;
        }

        public string AppSignature { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string SoapEndPoint { get; set; }

        public string AuthenticationEndPoint { get; set; }

        public string RestEndPoint { get; set; }

        public string UseOAuth2Authentication { get; set; }

        public string AccountId { get; set; }

        public string Scope { get; set; }
    }
}