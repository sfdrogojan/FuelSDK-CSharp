using System;

namespace FuelSDK
{
    internal class AuthEndpointUriBuilder
    {
        private const string legacyQuery = "legacy=1";
        private readonly IFuelSDKConfiguration configuration;

        public AuthEndpointUriBuilder(IFuelSDKConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Build()
        {
            UriBuilder uriBuilder = new UriBuilder(configuration.AuthenticationEndPoint);

            if (uriBuilder.Query.ToLower().Contains(legacyQuery))
            {
                return uriBuilder.Uri.AbsoluteUri;
            }

            if (uriBuilder.Query.Length > 1)
                uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + legacyQuery;
            else
                uriBuilder.Query = legacyQuery;

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}
