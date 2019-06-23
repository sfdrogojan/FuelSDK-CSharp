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

        public AuthService(IFuelSDKConfiguration configuration,
            IHttpWebRequestWrapperFactory httpWebRequestWrapperFactory)
        {
            this.configuration = configuration;
            this.httpWebRequestWrapperFactory = httpWebRequestWrapperFactory;
        }

        public bool UseOAuth2Authentication
        {
            get { return !String.IsNullOrEmpty(configuration.UseOAuth2Authentication) && Convert.ToBoolean(configuration.UseOAuth2Authentication); }
        }

        public void RefreshToken(bool force = false)
        {
            if (UseOAuth2Authentication)
            {
                RefreshTokenWithOauth2(force);
                return;
            }

            // workaround to support TLS 1.2 in .NET 4.0
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(configuration.AuthToken) && DateTime.Now.AddSeconds(300) <= configuration.AuthTokenExpiration && !force)
                return;

            // Get an internalAuthToken using clientId and clientSecret
            var authEndpoint = new AuthEndpointUriBuilder(configuration).Build();

            var httpWebRequestWrapper = this.httpWebRequestWrapperFactory.Create(new Uri(authEndpoint.Trim()));
            httpWebRequestWrapper.Method = "POST";
            httpWebRequestWrapper.ContentType = "application/json";
            httpWebRequestWrapper.UserAgent = configuration.SDKVersion;

            // Build the request
            //var request = (HttpWebRequest)WebRequest.Create(authEndpoint.Trim());
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //request.UserAgent = configuration.SDKVersion;

            using (var streamWriter = new StreamWriter(httpWebRequestWrapper.GetRequestStream()))
            {
                string json;
                if (!string.IsNullOrEmpty(configuration.RefreshKey))
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"refreshToken\": \"{2}\", \"scope\": \"cas:{3}\" , \"accessType\": \"offline\"}}", configuration.ClientId, configuration.ClientSecret, configuration.RefreshKey, configuration.InternalAuthToken);
                else
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"accessType\": \"offline\"}}", configuration.ClientId, configuration.ClientSecret);
                streamWriter.Write(json);
            }

            // Get the response
            string responseFromServer = httpWebRequestWrapper.GetResponse().GetContent();
            //using (var response = (HttpWebResponse)httpWebRequestWrapper.GetResponse())
            //using (var dataStream = response.GetResponseStream())
            //using (var reader = new StreamReader(dataStream))
            //    responseFromServer = reader.ReadToEnd();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            configuration.InternalAuthToken = parsedResponse["legacyToken"].Value<string>().Trim();
            configuration.AuthToken = parsedResponse["accessToken"].Value<string>().Trim();
            configuration.AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expiresIn"].Value<string>().Trim()));
            configuration.RefreshKey = parsedResponse["refreshToken"].Value<string>().Trim();
        }

        private void RefreshTokenWithOauth2(bool force = false)
        {
            // workaround to support TLS 1.2 in .NET 4.0
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(configuration.AuthToken) && DateTime.Now.AddSeconds(300) <= configuration.AuthTokenExpiration && !force)
                return;

            var authEndpoint = configuration.AuthenticationEndPoint + "/v2/token";

            var httpWebRequestWrapper = this.httpWebRequestWrapperFactory.Create(new Uri(authEndpoint.Trim()));
            httpWebRequestWrapper.Method = "POST";
            httpWebRequestWrapper.ContentType = "application/json";
            httpWebRequestWrapper.UserAgent = configuration.SDKVersion;

            //var request = (HttpWebRequest)WebRequest.Create(authEndpoint.Trim());
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //request.UserAgent = configuration.SDKVersion;

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
            //using (var response = (HttpWebResponse)httpWebRequestWrapper.GetResponse())
            //using (var dataStream = response.GetResponseStream())
            //using (var reader = new StreamReader(dataStream))
            //    responseFromServer = reader.ReadToEnd();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            configuration.AuthToken = parsedResponse["access_token"].Value<string>().Trim();
            configuration.InternalAuthToken = parsedResponse["access_token"].Value<string>().Trim();
            configuration.AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expires_in"].Value<string>().Trim()));
            configuration.SoapEndPoint = parsedResponse["soap_instance_url"].Value<string>().Trim() + "service.asmx";
            configuration.RestEndPoint = parsedResponse["rest_instance_url"].Value<string>().Trim();
        }
    }
}