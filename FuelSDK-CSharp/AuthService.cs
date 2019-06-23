using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("FuelSDK-Test")]
[assembly: InternalsVisibleTo("FuelSDK.Core.Test")]
namespace FuelSDK
{
    internal class AuthService : IAuthService
    {
        private readonly IFuelSDKConfiguration configuration;
        private readonly IHttpWebRequestWrapperFactory httpWebRequestWrapperFactory;
        private readonly ETClient etClient;

        public AuthService(IFuelSDKConfiguration configuration,
            IHttpWebRequestWrapperFactory httpWebRequestWrapperFactory,
            ETClient etClient)
        {
            this.configuration = configuration;
            this.httpWebRequestWrapperFactory = httpWebRequestWrapperFactory;
            this.etClient = etClient;
        }

        public void RefreshToken(bool force = false)
        {
            if (etClient.UseOAuth2Authentication)
            {
                RefreshTokenWithOauth2(force);
                return;
            }

            // workaround to support TLS 1.2 in .NET 4.0
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(etClient.AuthToken) && DateTime.Now.AddSeconds(300) <= etClient.AuthTokenExpiration && !force)
                return;

            // Get an internalAuthToken using clientId and clientSecret
            var authEndpoint = new AuthEndpointUriBuilder(configuration).Build();

            var httpWebRequestWrapper = this.httpWebRequestWrapperFactory.Create(new Uri(authEndpoint.Trim()));
            httpWebRequestWrapper.Method = "POST";
            httpWebRequestWrapper.ContentType = "application/json";
            httpWebRequestWrapper.UserAgent = ETClient.SDKVersion;

            // Build the request
            using (var streamWriter = new StreamWriter(httpWebRequestWrapper.GetRequestStream()))
            {
                string json;
                if (!string.IsNullOrEmpty(etClient.RefreshKey))
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"refreshToken\": \"{2}\", \"scope\": \"cas:{3}\" , \"accessType\": \"offline\"}}", configuration.ClientId, configuration.ClientSecret, etClient.RefreshKey, etClient.InternalAuthToken);
                else
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"accessType\": \"offline\"}}", configuration.ClientId, configuration.ClientSecret);
                streamWriter.Write(json);
            }

            // Get the response
            string responseFromServer = httpWebRequestWrapper.GetResponse().GetContent();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            etClient.InternalAuthToken = parsedResponse["legacyToken"].Value<string>().Trim();
            etClient.AuthToken = parsedResponse["accessToken"].Value<string>().Trim();
            etClient.AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expiresIn"].Value<string>().Trim()));
            etClient.RefreshKey = parsedResponse["refreshToken"].Value<string>().Trim();
        }

        private void RefreshTokenWithOauth2(bool force = false)
        {
            // workaround to support TLS 1.2 in .NET 4.0
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(etClient.AuthToken) && DateTime.Now.AddSeconds(300) <= etClient.AuthTokenExpiration && !force)
                return;

            var authEndpoint = configuration.AuthenticationEndPoint + "/v2/token";

            var httpWebRequestWrapper = this.httpWebRequestWrapperFactory.Create(new Uri(authEndpoint.Trim()));
            httpWebRequestWrapper.Method = "POST";
            httpWebRequestWrapper.ContentType = "application/json";
            httpWebRequestWrapper.UserAgent = ETClient.SDKVersion;

            using (var streamWriter = new StreamWriter(httpWebRequestWrapper.GetRequestStream()))
            {
                dynamic payload = new JObject();
                payload.client_id = configuration.ClientId;
                payload.client_secret = configuration.ClientSecret;
                payload.grant_type = "client_credentials";
                if (!string.IsNullOrEmpty(configuration.AccountId))
                    payload.account_id = configuration.AccountId;
                if (!string.IsNullOrEmpty(configuration.Scope))
                    payload.scope = configuration.Scope;

                streamWriter.Write(payload.ToString());
            }

            // Get the response
            string responseFromServer = httpWebRequestWrapper.GetResponse().GetContent();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            etClient.AuthToken = parsedResponse["access_token"].Value<string>().Trim();
            etClient.InternalAuthToken = parsedResponse["access_token"].Value<string>().Trim();
            etClient.AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expires_in"].Value<string>().Trim()));
            configuration.SoapEndPoint = parsedResponse["soap_instance_url"].Value<string>().Trim() + "service.asmx";
            configuration.RestEndPoint = parsedResponse["rest_instance_url"].Value<string>().Trim();
        }
    }
}